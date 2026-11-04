const fs = require('fs');
const path = require('path');
const crypto = require('crypto');

function getRandomScatteredBytes(filePath, byteCount = 256, targetCount = 10) {
  const fd = fs.openSync(filePath, "r");
  try {
    const fileSize = fs.fstatSync(fd).size;

    if (fileSize < byteCount) {
      throw new Error("File is smaller than the requested byte count.");
    }

    const chunk = Buffer.alloc(byteCount);
    let baseOffset;
    let attempts = 0;
    const maxAttempts = 100;

    do {
      baseOffset = Math.floor(Math.random() * (fileSize - byteCount + 1));
      fs.readSync(fd, chunk, 0, byteCount, baseOffset);
      attempts++;
      
      if (attempts >= maxAttempts) {
        break;
      }
    } while (!chunk.some(byte => byte !== 0));

    const selectedIndices = new Set();
    while (selectedIndices.size < targetCount) {
      const randomIndex = Math.floor(Math.random() * byteCount);
      selectedIndices.add(randomIndex);
    }

    const results = [];
    for (const localIndex of selectedIndices) {
      const absoluteOffset = baseOffset + localIndex;
      const byteValue = chunk[localIndex];

      results.push({
        offset: absoluteOffset,
        byte: byteValue
      });
    }

    return {
      baseOffset: baseOffset,
      blockSize: byteCount,
      randomBytes: results
    };
  } finally {
    fs.closeSync(fd);
  }
}

module.exports = async function(context) {
  const { appOutDir, electronPlatformName, packager } = context;
  const appName = packager.appInfo.productFilename;
  let targetFile = null;
  let metaFilePath = null;

  if (electronPlatformName === 'darwin') {
    targetFile = path.join(appOutDir, `${appName}.app`, 'Contents', 'MacOS', appName);
    metaFilePath = path.join(appOutDir, `${appName}.app`, 'Contents', 'Resources', 'app', 'app-meta.json');
  } else if (electronPlatformName === 'win32') {
    targetFile = path.join(appOutDir, `${appName}.exe`);
    metaFilePath = path.join(appOutDir, 'resources', 'app', 'app-meta.json');
  } else if (electronPlatformName === 'linux') {
    targetFile = path.join(appOutDir, appName);
    metaFilePath = path.join(appOutDir, 'resources', 'app', 'app-meta.json');
  }

  if (targetFile && fs.existsSync(targetFile)) {
    const stats = fs.statSync(targetFile);
    const sizeInBytes = stats.size;

    const packageJsonPath = path.join(packager.projectDir, 'package.json');
    const rootPkg = JSON.parse(fs.readFileSync(packageJsonPath, 'utf8'));
    const productName = rootPkg.build && rootPkg.build.productName ? rootPkg.build.productName : null;

    let metaData = {};

    if (fs.existsSync(metaFilePath)) {
      metaData = JSON.parse(fs.readFileSync(metaFilePath, 'utf8'));
    }

    if (productName) {
      metaData.productName = productName;
    }
    metaData.binarySize = sizeInBytes;

    metaData.randomBytesData = getRandomScatteredBytes(targetFile, 256, 10);

    const secretKey = "A1b2C3d4E5f6G7h8I9j0K1l2M3n4O5p6";
    const iv = crypto.randomBytes(12);
    const cipher = crypto.createCipheriv('aes-256-gcm', Buffer.from(secretKey, 'utf8'), iv);

    const jsonString = JSON.stringify(metaData);
    let encrypted = cipher.update(jsonString, 'utf8', 'base64');
    encrypted += cipher.final('base64');
    const authTag = cipher.getAuthTag();

    const combinedPayload = Buffer.concat([
      iv,
      Buffer.from(encrypted, 'base64'),
      authTag
    ]).toString('base64');

    fs.writeFileSync(metaFilePath, combinedPayload, 'utf8');

    const copiedScriptPath = path.join(path.dirname(metaFilePath), 'Scripts', 'after-pack.js');
    if (fs.existsSync(copiedScriptPath)) {
      fs.unlinkSync(copiedScriptPath);
    }
  }
};