const fs = require("fs");
const path = require("path");
const checker = require("license-checker");

const ROOT = process.cwd();

const OUTPUT_DIR = path.join(ROOT, "third-party-licenses");
const LICENSES_DIR = path.join(OUTPUT_DIR, "licenses");
const MARKDOWN_FILE = path.join(
  OUTPUT_DIR,
  "THIRD_PARTY_LICENSES.md"
);

if (path.resolve(OUTPUT_DIR) === path.resolve(ROOT)) {
  console.error(
    "ERROR: OUTPUT_DIR cannot be the same as ROOT."
  );

  console.error(
    "You must configure OUTPUT_DIR as a subdirectory of the project."
  );

  process.exit(1);
}

const EXCLUDED_PACKAGES = new Set([
  "electron-builder",
  "license-checker",
  "vite",
  "@vitejs/plugin-vue",
]);

const INCLUDED_DEV_PACKAGES = new Set([
  "electron",
]);

function cleanOutputDirectory() {
  fs.rmSync(OUTPUT_DIR, {
    recursive: true,
    force: true,
  });

  fs.mkdirSync(LICENSES_DIR, {
    recursive: true,
  });
}

function normalizeText(value) {
  if (!value) {
    return "";
  }

  return String(value)
    .replace(/\r\n/g, "\n")
    .replace(/\r/g, "\n")
    .trim();
}

function escapeMarkdown(value) {
  if (value === null || value === undefined) {
    return "";
  }

  return String(value)
    .replace(/\|/g, "\\|")
    .replace(/\r?\n/g, " ")
    .trim();
}

function sanitizeFileName(name) {
  return name
    .replace(/^@/, "")
    .replace(/[\/\\:*?"<>|]/g, "-")
    .replace(/\s+/g, "-")
    .toLowerCase();
}

function getPackageName(moduleName) {
  const lastAt = moduleName.lastIndexOf("@");

  if (lastAt > 0) {
    return moduleName.substring(0, lastAt);
  }

  return moduleName;
}

function getPackageVersion(moduleName, info) {
  if (info.version) {
    return info.version;
  }

  const lastAt = moduleName.lastIndexOf("@");

  if (lastAt > 0) {
    return moduleName.substring(lastAt + 1);
  }

  return "unknown";
}

function cleanRepositoryUrl(value) {
  if (!value) {
    return "";
  }

  return String(value)
    .replace(/^git\+/, "")
    .replace(/^git:\/\//, "https://")
    .replace(/\.git$/, "")
    .trim();
}

function getRepositoryUrl(info) {
  if (!info.repository) {
    return "";
  }

  if (typeof info.repository === "string") {
    return cleanRepositoryUrl(info.repository);
  }

  if (typeof info.repository.url === "string") {
    return cleanRepositoryUrl(info.repository.url);
  }

  return "";
}

function getPackageJson(info) {
  if (!info.licenseFile) {
    return null;
  }

  let currentDirectory = path.dirname(
    info.licenseFile
  );

  for (let i = 0; i < 6; i++) {
    const packageJsonPath = path.join(
      currentDirectory,
      "package.json"
    );

    try {
      if (fs.existsSync(packageJsonPath)) {
        return JSON.parse(
          fs.readFileSync(
            packageJsonPath,
            "utf8"
          )
        );
      }
    } catch {
      return null;
    }

    const parentDirectory =
      path.dirname(currentDirectory);

    if (
      parentDirectory === currentDirectory
    ) {
      break;
    }

    currentDirectory =
      parentDirectory;
  }

  return null;
}

function getAuthors(packageJson, info) {
  if (!packageJson) {
    return info.publisher || "";
  }

  if (packageJson.author) {
    if (typeof packageJson.author === "string") {
      return packageJson.author;
    }

    if (
      typeof packageJson.author === "object"
    ) {
      return [
        packageJson.author.name,
        packageJson.author.email
          ? `<${packageJson.author.email}>`
          : "",
      ]
        .filter(Boolean)
        .join(" ");
    }
  }

  if (
    Array.isArray(packageJson.authors)
  ) {
    return packageJson.authors
      .map((author) => {
        if (typeof author === "string") {
          return author;
        }

        if (
          author &&
          typeof author === "object"
        ) {
          return [
            author.name,
            author.email
              ? `<${author.email}>`
              : "",
          ]
            .filter(Boolean)
            .join(" ");
        }

        return "";
      })
      .filter(Boolean)
      .join(", ");
  }

  return info.publisher || "";
}

function extractCopyrightFromLicense(
  licenseText
) {
  if (!licenseText) {
    return "";
  }

  const matches =
    licenseText.match(
      /copyright[^\n]*/gi
    );

  if (!matches) {
    return "";
  }

  return matches
    .map((value) => value.trim())
    .join(" ");
}

function getCopyright(
  packageJson,
  licenseText
) {
  if (
    packageJson &&
    packageJson.copyright
  ) {
    return String(
      packageJson.copyright
    );
  }

  return extractCopyrightFromLicense(
    licenseText
  );
}

function getLicenseText(info) {
  if (info.licenseFile) {
    try {
      if (
        fs.existsSync(info.licenseFile)
      ) {
        const text =
          fs.readFileSync(
            info.licenseFile,
            "utf8"
          );

        if (normalizeText(text)) {
          return normalizeText(text);
        }
      }
    } catch {
    }
  }

  return normalizeText(
    info.licenseText
  );
}

function getLicenseUrl(license) {
  if (!license) {
    return "";
  }

  if (
    /(\sOR\s|\sAND\s|\||\/)/i.test(
      String(license)
    )
  ) {
    return "";
  }

  const normalized =
    String(license)
      .trim()
      .toUpperCase();

  const urls = {
    MIT:
      "https://opensource.org/license/mit/",
    "APACHE-2.0":
      "https://www.apache.org/licenses/LICENSE-2.0",
    "BSD-2-CLAUSE":
      "https://opensource.org/license/bsd-2-clause/",
    "BSD-3-CLAUSE":
      "https://opensource.org/license/bsd-3-clause/",
    ISC:
      "https://opensource.org/license/isc-license-txt/",
    "MPL-2.0":
      "https://www.mozilla.org/en-US/MPL/2.0/",
    "LGPL-2.1":
      "https://www.gnu.org/licenses/old-licenses/lgpl-2.1.html",
    "LGPL-3.0":
      "https://www.gnu.org/licenses/lgpl-3.0.html",
    "GPL-2.0":
      "https://www.gnu.org/licenses/old-licenses/gpl-2.0.html",
    "GPL-3.0":
      "https://www.gnu.org/licenses/gpl-3.0.html",
    "AGPL-3.0":
      "https://www.gnu.org/licenses/agpl-3.0.html",
    "CC0-1.0":
      "https://creativecommons.org/publicdomain/zero/1.0/",
    "0BSD":
      "https://opensource.org/license/0bsd/",
    ZLIB:
      "https://opensource.org/license/zlib/",
    UNLICENSE:
      "https://unlicense.org/",
  };

  return urls[normalized] || "";
}

function getLicenseOrigin(
  info,
  packageJson
) {
  if (
    packageJson &&
    packageJson.license
  ) {
    return "Expression";
  }

  if (info.licenseFile) {
    return "License File";
  }

  return "Unknown";
}

function scanStaticScriptsRecursively(dir, rootScriptsDir) {
  const staticPackages = [];

  if (!fs.existsSync(dir)) {
    return staticPackages;
  }

  const entries = fs.readdirSync(dir, { withFileTypes: true });

  entries.forEach((entry) => {
    const fullPath = path.join(dir, entry.name);
    const relativePath = path.relative(rootScriptsDir, fullPath);

    if (entry.isDirectory()) {
      const parts = relativePath.split(path.sep);
      if (parts[0].toLowerCase() === "pages") {
        return;
      }
      staticPackages.push(...scanStaticScriptsRecursively(fullPath, rootScriptsDir));
    } else if (entry.isFile()) {
      const parentDir = path.relative(rootScriptsDir, dir);
      if (!parentDir || parentDir === "") {
        return;
      }

      const parts = parentDir.split(path.sep);
      if (parts[0].toLowerCase() === "pages") {
        return;
      }

      if (entry.name.endsWith(".js") || entry.name.endsWith(".css")) {
        const content = fs.readFileSync(fullPath, "utf8");
        const commentMatch = content.match(/\/\*[\s\S]*?\*\//);
        let licenseText = commentMatch ? commentMatch[0] : "No license header found in file.";

        const subfolderName = parts[0].toLowerCase();
        const fileNameWithoutExt = entry.name
          .replace(/\.(min|bundle)?\.(js|css)$/, "")
          .toLowerCase();

        const packageName = `${subfolderName}-${fileNameWithoutExt}`;

        staticPackages.push({
          moduleName: packageName,
          info: {
            licenses: "MIT / Open Source",
            licenseText: licenseText,
            licenseFile: fullPath,
            version: "unknown",
            publisher: "External Subfolder Script",
            repository: "",
            url: ""
          }
        });
      }
    }
  });

  return staticPackages;
}

function scanStaticScripts() {
  const scriptsDir = path.join(ROOT, "scripts");
  return scanStaticScriptsRecursively(scriptsDir, scriptsDir);
}

function createLicenseFileContent({
  packageName,
  version,
  license,
  authors,
  copyright,
  projectUrl,
  licenseUrl,
  licenseText,
}) {
  const lines = [];

  lines.push(
    "=".repeat(80)
  );

  lines.push(
    `${packageName} ${version}`
  );

  lines.push(
    "=".repeat(80)
  );

  lines.push("");

  lines.push(
    `Package: ${packageName}`
  );

  lines.push(
    `Version: ${version}`
  );

  lines.push(
    `License: ${license || "UNKNOWN"}`
  );

  if (authors) {
    lines.push(
      `Authors: ${authors}`
    );
  }

  if (copyright) {
    lines.push(
      `Copyright: ${copyright}`
    );
  }

  if (projectUrl) {
    lines.push(
      `Project: ${projectUrl}`
    );
  }

  if (licenseUrl) {
    lines.push(
      `License URL: ${licenseUrl}`
    );
  }

  lines.push("");

  lines.push(
    "-".repeat(80)
  );

  lines.push("");

  if (licenseText) {
    lines.push(licenseText);
  } else {
    lines.push(
      "LICENSE TEXT NOT FOUND IN INSTALLED PACKAGE."
    );
  }

  lines.push("");

  return lines.join("\n");
}

function shouldIncludePackage(
  packageName,
  info
) {
  if (
    EXCLUDED_PACKAGES.has(
      packageName
    )
  ) {
    return false;
  }

  if (
    INCLUDED_DEV_PACKAGES.has(
      packageName
    )
  ) {
    return true;
  }

  return true;
}

checker.init(
  {
    start: ROOT,
    production: true,
  },
  (error, packages) => {
    if (error) {
      console.error(
        "Error running license-checker:"
      );

      console.error(error);

      process.exit(1);
    }

    checker.init(
      {
        start: ROOT,
        development: true,
      },
      (developmentError, developmentPackages) => {
        if (developmentError) {
          console.error(
            "Error getting development dependencies:"
          );

          console.error(
            developmentError
          );

          process.exit(1);
        }

        const allPackages = {
          ...packages,
        };

        for (
          const [
            moduleName,
            info,
          ] of Object.entries(
            developmentPackages
          )
        ) {
          const packageName =
            getPackageName(moduleName);

          if (
            INCLUDED_DEV_PACKAGES.has(
              packageName
            )
          ) {
            allPackages[moduleName] =
              info;
          }
        }

        const staticScripts = scanStaticScripts();
        staticScripts.forEach(item => {
          allPackages[item.moduleName] = item.info;
        });

        cleanOutputDirectory();

        const entries =
          Object.entries(
            allPackages
          )
            .filter(
              ([moduleName, info]) =>
                shouldIncludePackage(
                  getPackageName(
                    moduleName
                  ),
                  info
                )
            )
            .sort(
              ([a], [b]) =>
                a.localeCompare(b)
            );

        const markdown = [];

        markdown.push(
          "# Third Party Licenses"
        );

        markdown.push("");

        markdown.push(
          "This document contains license information for third-party dependencies included in this application."
        );

        markdown.push("");

        markdown.push(
          "| Package | Version | License Information Origin | License Expression | License Url | Copyright | Authors | Package Project Url |"
        );

        markdown.push(
          "|---|---|---|---|---|---|---|---|"
        );

        let unknownLicenses = 0;
        let missingLicenseText = 0;

        for (
          const [
            moduleName,
            info,
          ] of entries
        ) {
          const packageName =
            getPackageName(
              moduleName
            );

          const version =
            getPackageVersion(
              moduleName,
              info
            );

          const license =
            info.licenses ||
            "UNKNOWN";

          const licenseText =
            getLicenseText(info);

          const packageJson =
            getPackageJson(info);

          const authors =
            getAuthors(
              packageJson,
              info
            );

          const copyright =
            getCopyright(
              packageJson,
              licenseText
            );

          const projectUrl =
            getRepositoryUrl(info) ||
            info.url ||
            "";

          const licenseUrl =
            getLicenseUrl(
              license
            );

          const licenseOrigin =
            getLicenseOrigin(
              info,
              packageJson
            );

          if (
            !license ||
            license === "UNKNOWN"
          ) {
            unknownLicenses++;
          }

          if (!licenseText) {
            missingLicenseText++;
          }

          const fileName =
            `${sanitizeFileName(
              packageName
            )}.txt`;

          const filePath =
            path.join(
              LICENSES_DIR,
              fileName
            );

          const fileContent =
            createLicenseFileContent({
              packageName,
              version,
              license,
              authors,
              copyright,
              projectUrl,
              licenseUrl,
              licenseText,
            });

          fs.writeFileSync(
            filePath,
            fileContent,
            "utf8"
          );

          markdown.push(
            `| ${escapeMarkdown(packageName)} | ${escapeMarkdown(version)} | ${escapeMarkdown(licenseOrigin)} | ${escapeMarkdown(license)} | ${escapeMarkdown(licenseUrl)} | ${escapeMarkdown(copyright)} | ${escapeMarkdown(authors)} | ${escapeMarkdown(projectUrl)} |`
          );
        }

        markdown.push("");

        markdown.push(
          `Packages: ${entries.length}`
        );

        markdown.push("");

        markdown.push(
          "Complete license texts are available in the `licenses` directory."
        );

        fs.writeFileSync(
          MARKDOWN_FILE,
          markdown.join("\n"),
          "utf8"
        );

        console.log("");
        console.log(
          "Third-party licenses generated successfully."
        );
        console.log("");
        console.log(
          `Packages: ${entries.length}`
        );
        console.log(
          `Unknown licenses: ${unknownLicenses}`
        );
        console.log(
          `Missing license text: ${missingLicenseText}`
        );
        console.log("");
        console.log(
          `Output: ${OUTPUT_DIR}`
        );
        console.log("");

        if (
          unknownLicenses > 0 ||
          missingLicenseText > 0
        ) {
          console.log(
            "Some packages require manual license review before publishing."
          );
        }
      }
    );
  }
);