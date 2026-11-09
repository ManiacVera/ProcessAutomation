// windows.js
const { BrowserWindow, ipcMain } = require('electron');
const path = require('path');

function createDialogWindow(parentWindow, options) {
  return new Promise((resolve) => {
    const showOverlay = () => {
      if (parentWindow.isDestroyed()) {
        return;
      }

      parentWindow.webContents.executeJavaScript(`
        (() => {
          const id = '__electron_modal_overlay__';

          let overlay = document.getElementById(id);

          if (!overlay) {
            overlay = document.createElement('div');
            overlay.id = id;

            Object.assign(overlay.style, {
              position: 'fixed',
              left: '0',
              top: '0',
              width: '100vw',
              height: '100vh',
              background: 'rgba(0, 0, 0, 0.45)',
              zIndex: '2147483647',
              pointerEvents: 'none',
              boxSizing: 'border-box'
            });

            document.documentElement.appendChild(overlay);
          }

          overlay.style.display = 'block';
        })();
      `).catch(() => {});
    };

    const hideOverlay = () => {
      if (parentWindow.isDestroyed()) {
        return;
      }

      parentWindow.webContents.executeJavaScript(`
        document.getElementById('__electron_modal_overlay__')?.remove();
      `).catch(() => {});
    };

    showOverlay();

    const dialogWindow = new BrowserWindow({
      width: 420,
      height: 240,
      frame: false,
      resizable: false,
      parent: parentWindow,
      modal: true,
      webPreferences: {
        nodeIntegration: false,
        contextIsolation: true,
        preload: path.join(__dirname, 'preload.js')
      }
    });

    dialogWindow.loadFile('dist/dialog.html');

    if (process.platform === 'linux' && process.env.WAYLAND_DISPLAY) {
      dialogWindow.on('blur', () => {
        if (!dialogWindow.isDestroyed()) {
          dialogWindow.focus();
        }
      });
    }

    dialogWindow.webContents.on('did-finish-load', () => {
      dialogWindow.webContents.send('dialog-init', options);
    });

    const cleanup = () => {
      hideOverlay();

      if (!dialogWindow.isDestroyed()) {
        dialogWindow.close();
      }
    };

    const responseHandler = (event, responseIndex) => {
      ipcMain.removeListener('dialog-result', responseHandler);

      cleanup();

      resolve(responseIndex);
    };

    ipcMain.on('dialog-result', responseHandler);

    dialogWindow.on('closed', () => {
      ipcMain.removeListener('dialog-result', responseHandler);

      hideOverlay();

      resolve(null);
    });
  });
}

async function showConfirm(message, title = "Process Automation", buttons = ["Accept", "Cancel"]) {
  const resultIndex = await createDialogWindow
  (
    global.mainWindow,
    {
      title,
      message,
      buttons
    }
  );
  return resultIndex === 0;
}

async function showAlert(message, title = "Process Automation", buttons = ["OK"]) {
  const resultIndex = await createDialogWindow
  (
    global.mainWindow,
    {
      title,
      message,
      buttons
    }
  );
  return resultIndex;
}

function openPopup(mainWindow, page, data) {
  console.log(data);
  let popup = new BrowserWindow({
    parent: mainWindow,
    modal: true,
    frame: false,
    webPreferences: {
      preload: __dirname + '/preload.js',
      nodeIntegration: false,
      contextIsolation: true
    }
  });

  popup.maximize();
  popup.loadFile(page);

  if (process.env.DEBUG_MODE === 'true') {
    popup.webContents.on('before-input-event', (event, input) => {
      if (input.control && input.shift && input.key.toLowerCase() === 'i') {
        popup.webContents.openDevTools();
      }
    });
  }

  popup.webContents.on('did-finish-load', () => {
    popup.webContents.send('on-popup-data', data);
  });

  popup.on('closed', () => {
    popup = null;
  });
}

module.exports = { showConfirm, showAlert, openPopup };