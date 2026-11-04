const { ipcMain } = require('electron');
const path = require('path');
const { getSystemToken } = require(path.join(__dirname, 'systemToken.js'));

function setupIpc() {
  const WEB_API_BASE = global.webApi;

  // WEB API Endpoints
  const testPingUrl = `${WEB_API_BASE}/Test/Ping`;

  const openProcessUrl = `${WEB_API_BASE}/Process/OpenProcess`;
  const saveProcessUrl = `${WEB_API_BASE}/Process/SaveProcess`;
  const buildProcessUrl = `${WEB_API_BASE}/Process/BuildProcess`;
  const retrieveProcessListUrl = `${WEB_API_BASE}/Process/RetrieveProcessList`;
  const retrieveComponentUrl = `${WEB_API_BASE}/Process/RetrieveComponent`;
  const retrieveDeclarationTypeUrl = `${WEB_API_BASE}/Process/RetrieveDeclarationType`;
  const retrieveValueAccessUrl = `${WEB_API_BASE}/Process/RetrieveValueAccess`;
  const retrieveValueTypeUrl = `${WEB_API_BASE}/Process/RetrieveValueType`;
  const retrieveValueOutputTypeUrl = `${WEB_API_BASE}/Process/RetrieveValueOutputType`;
  const retrieveEncapsulatedPartitionTypeUrl = `${WEB_API_BASE}/Process/RetrieveEncapsulatedPartitionType`;
  const retrieveOutputValueOptionUrl = `${WEB_API_BASE}/Process/RetrieveOutputValueOption`;
  const retrieveLanguageListUrl = `${WEB_API_BASE}/Process/RetrieveLanguageList`;
  const buildInputFormatUrl = `${WEB_API_BASE}/Process/BuildInputFormat`;
  const buildResultValueFormatUrl = `${WEB_API_BASE}/Process/BuildResultValueFormat`;
  const loadUserSettingsUrl = `${WEB_API_BASE}/Process/LoadUserSettings`;
  const saveUserSettingsUrl = `${WEB_API_BASE}/Process/SaveUserSettings`;
  const getDataPathUrl = `${WEB_API_BASE}/Process/GetDataPath`;

  const setDefaultSettingsUrl = `${WEB_API_BASE}/RunProcess/SetDefaultSettings`;
  const initProcessUrl = `${WEB_API_BASE}/RunProcess/InitProcess`;
  const getStepDescriptionUrl = `${WEB_API_BASE}/RunProcess/GetStepDescription`;
  const getProgressUrl = `${WEB_API_BASE}/RunProcess/GetProgress`;
  const getMaximumProgressUrl = `${WEB_API_BASE}/RunProcess/GetMaximumProgress`;
  const isRunningUrl = `${WEB_API_BASE}/RunProcess/IsRunning`;
  const abortProcessUrl = `${WEB_API_BASE}/RunProcess/AbortProcess`;
  const setInputValueUrl = `${WEB_API_BASE}/RunProcess/SetInputValue`;
  const runProcessUrl = `${WEB_API_BASE}/RunProcess/RunProcess`;

  const wait = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

  async function secureFetch(fullUrl, options = {}) {
    const urlObject = new URL(fullUrl);
    const protocol = urlObject.protocol.replace(':', '');
    const ip = urlObject.hostname;
    const port = urlObject.port || (protocol === 'https' ? '443' : '80');

    const token = await getSystemToken(protocol, ip, port);

    const headers = {
        'Content-Type': 'application/json',
        'X-Token': token,
        ...(options.headers || {})
    };

    return await fetch(fullUrl, {
        ...options,
        headers
    });
  }

  async function setDefaultSettings(settings) {
    try {
      await secureFetch(setDefaultSettingsUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(settings)
      });
      return null;
    } catch (err) {
      console.error('Error in set default settings (Main):', err);
      return null;
    }
  }

  ipcMain.handle('api:setDefaultSettings', async (event, settings) => {
    return await setDefaultSettings(settings);
  });

  async function testPing() {
      try {
          const response = await secureFetch(testPingUrl);
          return await response.text() === "true";
      } catch (err) {
          console.error('Error in test ping (Main):', err);
          return false;
      }
  }

  async function waitForPing(intervalMs = 1000, maxAttempts = 30) {
      let attempts = 0;

      while (attempts < maxAttempts) {
          try {
              const isAlive = await testPing();
              if (isAlive) {
                  return true;
              }
          } catch (error) {
              console.warn(`Attempt ${attempts + 1} failed, retrying...`);
          }

          attempts++;
          await wait(intervalMs);
      }

      return false;
  }

  ipcMain.handle('api:openProcess', async (event, processFile, userCode, directory) => {
    try {

      const b64ProcessFile = encodeURIComponent(Buffer.from(processFile).toString('base64'));
      const b64UserCode = encodeURIComponent(Buffer.from(userCode).toString('base64'));
      const b64Directory = encodeURIComponent(Buffer.from(directory).toString('base64'));

      const response = await secureFetch(`${openProcessUrl}/${b64ProcessFile}/${b64UserCode}/${b64Directory}`);
      const processInfo = await response.json();

      return {
        UserCode: userCode,
        BaseDirectory: directory,
        ProcessFile: processFile,
        ProcessData: processInfo
      };
    } catch (err) {
      console.error('Error cargando componentes (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:saveProcess', async (event, data) => {
    try {
      if (data) {
        const response = await secureFetch(saveProcessUrl, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(data)
        });
        return await response.text();
      }
      return null;
    } catch (err) {
      console.error('Error on save process (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:buildProcess', async (event, data) => {
    try {
      if (data) {
        const response = await secureFetch(buildProcessUrl, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(data)
        });
        return await response.text();
      }
      return null;
    } catch (err) {
      console.error('Error on build process (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:retrieveProcessList', async (event, userCode, directory) => {
    try {
      const b64UserCode = encodeURIComponent(Buffer.from(userCode).toString('base64'));
      const b64Directory = encodeURIComponent(Buffer.from(directory).toString('base64'));

      const response = await secureFetch(`${retrieveProcessListUrl}/${b64UserCode}/${b64Directory}`);
      return await response.json();
    } catch (err) {
      console.error('Error in retrieve process list (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:retrieveComponent', async () => {
    try {
      const response = await secureFetch(retrieveComponentUrl);
      return await response.json();
    } catch (err) {
      console.error('Error in retrieve component (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:retrieveDeclarationType', async () => {
    try {
      const response = await secureFetch(retrieveDeclarationTypeUrl);
      return await response.json();
    } catch (err) {
      console.error('Error in retrieve declaration type (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:retrieveValueAccess', async () => {
    try {
      const response = await secureFetch(retrieveValueAccessUrl);
      return await response.json();
    } catch (err) {
      console.error('Error in value access (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:retrieveValueType', async () => {
    try {
      const response = await secureFetch(retrieveValueTypeUrl);
      return await response.json();
    } catch (err) {
      console.error('Error in retrieve value type (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:retrieveValueOutputType', async () => {
    try {
      const response = await secureFetch(retrieveValueOutputTypeUrl);
      return await response.json();
    } catch (err) {
      console.error('Error in retrieve value output type (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:retrieveEncapsulatedPartitionType', async () => {
    try {
      const response = await secureFetch(retrieveEncapsulatedPartitionTypeUrl);
      return await response.json();
    } catch (err) {
      console.error('Error in retrieve encapsulated (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:retrieveOutputValueOption', async (event, data) => {
    try {
      const response = await secureFetch(retrieveOutputValueOptionUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: data
      });
      return await response.json();
    } catch (err) {
      console.error('Error on retrieve output value (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:retrieveLanguageList', async () => {
    try {
      const response = await secureFetch(retrieveLanguageListUrl);
      return await response.json();
    } catch (err) {
      console.error('Error in retrieve language list (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:buildInputFormat', async (event, buildDataInfo) => {
    try {
      if (buildDataInfo) {
        const response = await secureFetch(buildInputFormatUrl, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(buildDataInfo)
        });
        return await response.json();
      }
      return null;
    } catch (err) {
      console.error('Error on build format (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:initProcess', async (event, processData) => {
    try {
      if (processData) {
        const response = await secureFetch(initProcessUrl, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(processData)
        });
        return await response.json();
      }
      return null;
    } catch (err) {
      console.error('Error on init process (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:getStepDescription', async () => {
    try {
      const response = await secureFetch(getStepDescriptionUrl);
      return await response.json();
    } catch (err) {
      console.error('Error on step description (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:getProgress', async () => {
    try {
      const response = await secureFetch(getProgressUrl);
      return parseInt(await response.text(), 10);
    } catch (err) {
      console.error('Error on getProgress (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:getMaximumProgress', async () => {
    try {
      const response = await secureFetch(getMaximumProgressUrl);
      return parseInt(await response.text(), 10);
    } catch (err) {
      console.error('Error on maximum progress (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:isRunning', async () => {
    try {
      const response = await secureFetch(isRunningUrl);
      return await response.text() === "true";
    } catch (err) {
      console.error('Error in is running (Main):', err);
      return false;
    }
  });

  ipcMain.handle('api:abortProcess', async () => {
    try {
      await secureFetch(abortProcessUrl, { method: 'POST' });
      return null;
    } catch (err) {
      console.error('Error in abort (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:setInputValue', async (event, data) => {
    try {
      await secureFetch(setInputValueUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
      });
      return null;
    } catch (err) {
      console.error('Error in set input value (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:runProcess', async () => {
    try {
      await secureFetch(runProcessUrl, { method: 'POST' });
      return null;
    } catch (err) {
      console.error('Error in run process (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:loadUserSettings', async () => {
    try {
      const response = await secureFetch(loadUserSettingsUrl);
      return await response.json();
    } catch (err) {
      console.error('Error in load user settings (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:saveUserSettings', async (event, data) => {
    try {
      if (data) {
        const response = await secureFetch(saveUserSettingsUrl, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(data)
        });
        return await response.text() === "true";
      }
      return false;
    } catch (err) {
      console.error('Error on save user settings (Main):', err);
      return false;
    }
  });

  ipcMain.handle('api:getDataPath', async () => {
    try {
      const response = await secureFetch(getDataPathUrl);
      return await response.text();
    } catch (err) {
      console.error('Error in get data folder (Main):', err);
      return null;
    }
  });

  ipcMain.handle('api:buildResultValueFormat', async (event, buildDataInfo) => {
    try {
      if (buildDataInfo) {
        const response = await secureFetch(buildResultValueFormatUrl, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(buildDataInfo)
        });

        const result = await response.text();
        return result;
      }
      return null;
    } catch (err) {
      console.error('Error on build result value format (Main):', err);
      return null;
    }
  });

  return {
    waitForPing,
    setDefaultSettings
  };
}

module.exports = setupIpc;