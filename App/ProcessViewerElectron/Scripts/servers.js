const { app, BrowserWindow, ipcMain, screen } = require('electron');
const path = require('path');
const fs = require('fs').promises;
const net = require('net');
const { spawn } = require('child_process');
const crypto = require('crypto');

let apiProcess;
let logWindow = null;
let logStore = [];

function getFreePort(options = {}) {
  const { startPort, endPort = 65535 } = options;
  
  return new Promise((resolve, reject) => {
    if (typeof startPort !== 'number') {
      const server = net.createServer();
      server.listen(0, () => {
        const { port } = server.address();
        server.close(() => resolve(port));
      });
      return;
    }
    
    function tryPort(port) {
      if (port > endPort) return reject(new Error('No free ports available'));
      const server = net.createServer();
      server.once('error', () => tryPort(port + 1));
      server.once('listening', () => server.close(() => resolve(port)));
      server.listen(port);
    }
    tryPort(startPort);
  });
}

async function createServerApi() {
  const debug_api = `http://localhost:5004`;
  const port = await getFreePort({ startPort: 5010 });
  
  if (process.env.DEBUG_MODE === 'true') {
    global.webApi = debug_api;
  }
  else {
    global.webApi = `http://127.0.0.1:${port}`;
  }

  let executable;
  switch (process.platform) {
    case 'win32': executable = 'ProcessAutomationApi.exe'; break;
    case 'darwin': 
    case 'linux': executable = 'ProcessAutomationApi'; break;
  }

  if (global.webApi !== debug_api) { 
    const apiPath = path.join(__dirname, '..', 'Core', executable);
    apiProcess = spawn(apiPath, [], {
      env: { ...process.env, ASPNETCORE_URLS: global.webApi },
      stdio: 'pipe'
    });

    apiProcess.stdout.on('data', d => console.log(`API: ${d.toString()}`));
    apiProcess.stderr.on('data', d => console.error(`API Error: ${d.toString()}`));
    apiProcess.on('exit', (code, signal) => console.log(`API exited with ${code}, signal ${signal}`));

    app.on('before-quit', () => {
      if (apiProcess) apiProcess.kill();
    });
  }
}

function generatePipeName() {
  const randomId = crypto.randomBytes(8).toString('hex');
  return process.platform === 'win32'
    ? `\\\\.\\pipe\\electron_pipe_${randomId}`
    : `/tmp/electron_pipe_${randomId}.sock`;
}

function openLogWindow(logData, socket, mainWindow) {
  console.log("openLogWindow");
  const htmlPath = path.join(__dirname, '..', 'Pages', 'LogPage.html');

  const sendOk = () => {
    if (socket && !socket.destroyed) {
      socket.write("OK\n");
    }
  };

  if (logData) {
    logStore.push(logData);
  }
  console.log(logData);

  if (logWindow && !logWindow.isDestroyed()) {
    // if (logWindow.isMinimized()) {
    //   logWindow.restore();
    // }

    logWindow.webContents.send('log-data', logData);
    sendOk();
    return;
  }

  const { width, height } = screen.getPrimaryDisplay().workAreaSize;

  logWindow = new BrowserWindow({
    parent: mainWindow,
    modal: false,
    x: 0,
    y: Math.floor(height * 0.7),
    width: width,
    height: Math.floor(height * 0.3),
    title: "Log Application",
    alwaysOnTop: true,
    frame: true,
    resizable: true,
    minimizable: true,
    maximizable: false,
    skipTaskbar: false,
    show: false,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js')
    }
  });

  logWindow.loadFile(htmlPath);

  logWindow.once('ready-to-show', () => {
    logWindow.show();
    logWindow.webContents.send('log-data', logStore);
    sendOk();
  });

  logWindow.on('closed', () => {
    logWindow = null;
  });
}

async function createServerPipe(mainWindow, pipeName) {
  if (process.platform !== 'win32') {
    try { await fs.access(pipeName); await fs.unlink(pipeName); } catch {}
  }

  const server = net.createServer(socket => {
    socket.on('data', async data => {
      const message = data.toString().trim();
      if (message.startsWith('OPEN_DIALOG::')) {
        const htmlPath = message.substring('OPEN_DIALOG::'.length).trim();
        let popup = new BrowserWindow({
          width: 500,
          height: 400,
          parent: mainWindow,
          modal: true,
          frame: false,
          webPreferences: { preload: path.join(__dirname, 'preload.js') }
        });

        const absolutePath = path.isAbsolute(htmlPath) ? htmlPath : path.join(__dirname, htmlPath);
        popup.maximize();
        popup.loadFile(absolutePath);

        let responseSent = false;
        const dialogHandler = (event, value) => {
          if (event.sender === popup.webContents && !responseSent) {
            responseSent = true;
            socket.write(value + '\n');
            popup.close();
          }
        };

        ipcMain.on('dialog-result', dialogHandler);

        popup.on('closed', () => {
          if (!responseSent) socket.write('CANCELED\n');
          ipcMain.removeListener('dialog-result', dialogHandler);
          socket.end();
        });
      }
      else if (message.startsWith('SEND_LOG_STRING::')) {
        const logData = message.substring('SEND_LOG_STRING::'.length).trim();
        openLogWindow(logData, socket, mainWindow);
      }
    });
  });

  server.on('error', err => console.error('Pipe error:', err.message));
  server.listen(pipeName, () => console.log(`Electron server listening on ${pipeName}`));

  app.on('before-quit', () => { if (server) server.close(); });
}

module.exports = { createServerApi, createServerPipe, generatePipeName };