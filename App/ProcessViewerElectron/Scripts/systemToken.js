const { execFile } = require('child_process');
const path = require('path');
const util = require('util');

const execFileAsync = util.promisify(execFile);

// Executes the Rust binary to generate and register the security token.
async function getSystemToken(protocol = 'https', ip = '127.0.0.1', port = '5001') {
    // Determine the correct executable name based on the operating system
    const isWindows = process.platform === 'win32';
    const binaryName = isWindows ? 'system_token.exe' : 'system_token';
    const binaryPath = path.join(__dirname, '..', binaryName);

    try {
        const { stdout, stderr } = await execFileAsync(binaryPath, [protocol, ip, port]);

        if (stderr && stderr.trim() !== '') {
            console.warn('Rust binary warning:', stderr.trim());
        }

        return stdout.trim();
    } catch (error) {
        console.error('Failed to execute the token generator binary:', error);
        throw new Error('Token generation failed.');
    }
}

module.exports = { getSystemToken };