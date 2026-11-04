const fs = require('fs-extra');
const path = require('path');

const apiDestDir = path.join(__dirname, '../Core');
const apiSourceDir = path.join(__dirname, '../../../Build/App/ProcessAutomationApi');
const apiExclude = ['Component', 'Viewer', 'External', 'ProcessFiles', 'Resources', 'runtimes', 'ProcessAutomationApi.deps.json', 'ProcessAutomationApi.pdb', 'ProcessAutomationApi.runtimeconfig.json', 'ProcessAutomationApi.staticwebassets.endpoints.json'];

const scriptsDestDir = path.join(__dirname, '../Scripts');
const scriptsSourceDir = path.join(__dirname, '../../../Build/Resources/Scripts');

const formDestDir = path.join(__dirname, '../Resources');
const inputFieldsSourceDir = path.join(__dirname, '../../../Common/ProcessAutomation.Common/Form/InputFields.html');
const inputTextFieldSourceDir = path.join(__dirname, '../../../Common/ProcessAutomation.Common/Form/InputTextField.html');
const outputValueOptionSourceDir = path.join(__dirname, '../../../Common/ProcessAutomation.Common/Form/OutputValueOption.html');

const systemTokenSourceDir = path.join(__dirname, '../../../Build/App/SystemTokenRust');
const systemTokenDestDir = path.join(__dirname, '..');

function copyFiles(destDir, sources, exclude = []) {
  try {
    fs.ensureDirSync(destDir);
    const items = [].concat(sources);

    items.forEach(source => {
      const stats = fs.statSync(source);

      if (stats.isFile()) {
        const fileName = path.basename(source);

        if (exclude.includes(fileName)) return;

        const finalPath = path.join(destDir, fileName);
        fs.copySync(source, finalPath);
      } 
      else if (stats.isDirectory()) {
        const files = fs.readdirSync(source);

        files.forEach(file => {
          const srcPath = path.join(source, file);
          const destPath = path.join(destDir, file);

          const relativePath = path.relative(source, srcPath)
            .replace(/\\/g, '/');

          const shouldExclude = exclude.some(ex => {
            const exNorm = ex.replace(/\\/g, '/');

            return (
              relativePath === exNorm ||
              relativePath.startsWith(exNorm + '/') ||
              file === exNorm
            );
          });

          if (shouldExclude) return;

          fs.copySync(srcPath, destPath);
        });
      }
    });

    console.log('Copy completed');
  } catch (err) {
    console.error('Error copying files:', err);
  }
}

copyFiles(apiDestDir, apiSourceDir, apiExclude);
copyFiles(scriptsDestDir, scriptsSourceDir);
copyFiles(formDestDir, [inputFieldsSourceDir, inputTextFieldSourceDir, outputValueOptionSourceDir]);
copyFiles(systemTokenDestDir, systemTokenSourceDir);