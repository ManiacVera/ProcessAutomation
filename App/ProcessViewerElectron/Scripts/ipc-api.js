
const { ipcMain } = require('electron');
const path = require('path');
const { getSystemToken } = require(path.join(__dirname, 'systemToken.js'));

let i18n;

function setupIpc(mainWindow) {

    const WEB_API_BASE = global.webApi;

    // WEB API Endpoints
    const testPingUrl = `${WEB_API_BASE}/Test/Ping`;
    const setDefaultSettingsUrl = `${WEB_API_BASE}/ProcessViewer/SetDefaultSettings`;
    const setWorkSpaceDirectoryUrl = `${WEB_API_BASE}/ProcessViewer/SetWorkSpaceDirectory`;
    const retrieveLanguageListUrl = `${WEB_API_BASE}/ProcessViewer/RetrieveLanguageList`;
    const reloadListUrl = `${WEB_API_BASE}/ProcessViewer/ReloadList`;
    const retrieveProcessViewerListUrl = `${WEB_API_BASE}/ProcessViewer/RetrieveProcessViewerList`;
    const regenerateConfigFileUrl = `${WEB_API_BASE}/ProcessViewer/RegenerateConfigFile`;
    const isRunningUrl = `${WEB_API_BASE}/ProcessViewer/IsRunning`;
    const stopProgramUrl = `${WEB_API_BASE}/ProcessViewer/StopProgram`;
    const saveConfigUrl = `${WEB_API_BASE}/ProcessViewer/SaveConfig`;
    const saveProcessItemUrl = `${WEB_API_BASE}/ProcessViewer/SaveProcessItem`;
    const copyProcessByNumberUrl = `${WEB_API_BASE}/ProcessViewer/CopyProcessByNumber`;
    const createProcessShortcutByNumberUrl = `${WEB_API_BASE}/ProcessViewer/CreateProcessShortcutByNumber`;
    const deleteProcessByNumberUrl = `${WEB_API_BASE}/ProcessViewer/DeleteProcessByNumber`;
    const runProcessListUrl = `${WEB_API_BASE}/ProcessViewer/RunProcessList`;
    const getProgressUrl = `${WEB_API_BASE}/ProcessViewer/GetProgress`;
    const getMaximumProgressUrl = `${WEB_API_BASE}/ProcessViewer/GetMaximumProgress`;
    const getLogUrl = `${WEB_API_BASE}/ProcessViewer/GetLog`;
    const readProcessFileUrl = `${WEB_API_BASE}/ProcessViewer/ReadProcessFile`;
    const saveProcessConfigUrl = `${WEB_API_BASE}/ProcessViewer/SaveProcessConfig`;
    const loadUserSettingsUrl = `${WEB_API_BASE}/ProcessViewer/LoadUserSettings`;
    const saveUserSettingsUrl = `${WEB_API_BASE}/ProcessViewer/SaveUserSettings`;
    const buildInputFormatUrl = `${WEB_API_BASE}/ProcessViewer/BuildInputFormat`;
    const buildResultValueFormatUrl = `${WEB_API_BASE}/ProcessViewer/BuildResultValueFormat`;

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

    ipcMain.handle('api:setWorkSpaceDirectory', async (event, directory) => {
        try {
            await secureFetch(setWorkSpaceDirectoryUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ 'Text': directory })
            });
            return null;
        } catch (err) {
            console.error('Error in set base directory (Main):', err);
            return null;
        }
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

    ipcMain.handle('api:retrieveLanguageList', async () => {
        try {
            const response = await secureFetch(retrieveLanguageListUrl);
            return await response.json();
        } catch (err) {
            console.error('Error in retrieve language list (Main):', err);
            return null;
        }
    });

    ipcMain.handle('api:reloadList', async () => {
        try {
            await secureFetch(reloadListUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });
            return null;
        } catch (err) {
            console.error('Error on reloadList (Main):', err);
            return null;
        }
    });

    ipcMain.handle('api:retrieveProcessViewerList', async () => {
        try {
            const response = await secureFetch(retrieveProcessViewerListUrl);
            return await response.json();
        } catch (err) {
            console.error('Error in retrieve process viewer list (Main):', err);
            return null;
        }
    });

    ipcMain.handle('api:regenerateConfigFile', async () => {
        try {
            await secureFetch(regenerateConfigFileUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });
            return null;
        } catch (err) {
            console.error('Error on reloadList (Main):', err);
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

    ipcMain.handle('api:stopProgram', async () => {
        try {
            await secureFetch(stopProgramUrl, { method: 'POST' });
            return null;
        } catch (err) {
            console.error('Error in stop program (Main):', err);
            return null;
        }
    });

    ipcMain.handle('api:saveConfig', async (event, data) => {
        try {
            await secureFetch(saveConfigUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            return null;
        } catch (err) {
            console.error('Error on run process (Main):', err);
            return null;
        }
    });

    ipcMain.handle('api:saveProcessItem', async (event, data) => {
        try {
            const response = await secureFetch(saveProcessItemUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            return await response.text() === "true";
        } catch (err) {
            console.error('Error on save process (Main):', err);
            return false;
        }
    });

    ipcMain.handle('api:copyProcessByNumber', async (event, data) => {
        try {
            await secureFetch(copyProcessByNumberUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            return null;
        } catch (err) {
            console.error('Error on copy process (Main):', err);
            return null;
        }
    });

    ipcMain.handle('api:createProcessShortcutByNumber', async (event, data) => {
        try {
            await secureFetch(createProcessShortcutByNumberUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            return null;
        } catch (err) {
            console.error('Error on create shortcut (Main):', err);
            return null;
        }
    });

    ipcMain.handle('api:deleteProcessByNumber', async (event, data) => {
        try {
            await secureFetch(deleteProcessByNumberUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            return null;
        } catch (err) {
            console.error('Error on delete process (Main):', err);
            return null;
        }
    });

    ipcMain.handle('api:runProcessList', async (event, data) => {
        try {
            const response = await secureFetch(runProcessListUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            return await response.text();
        } catch (err) {
            console.error('Error on run process (Main):', err);
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

    ipcMain.handle('api:getLog', async () => {
        try {
            const response = await secureFetch(getLogUrl);
            return await response.json();
        } catch (err) {
            console.error('Error in get log (Main):', err);
            return null;
        }
    });

    ipcMain.handle('api:readProcessFile', async (event, processFile) => {
        try {
            const response = await secureFetch(`${readProcessFileUrl}/${encodeURIComponent(processFile)}`);
            return await response.json();
        } catch (err) {
            console.error('Error in get log (Main):', err);
            return null;
        }
    });

    ipcMain.handle('api:saveProcessConfig', async (event, data) => {
        try {
            await secureFetch(saveProcessConfigUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            return null;
        } catch (err) {
            console.error('Error in saveProcessConfig (Main):', err);
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
            console.error('Error on build process (Main):', err);
            return false;
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