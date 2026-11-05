use std::fs;
use std::io::{self, Write};
use std::path::Path;
use std::process::Command;

fn main() {
    let os = std::env::consts::OS;
    let arch = std::env::consts::ARCH;
    println!("Detected Operating System: {} ({})", os, arch);

    println!("\n--- INSTALLATION MENU ---");
    println!("1. Minimalist Installation (Without MAUI) - (RECOMMENDED)");
    println!("2. Full Installation (Includes .NET MAUI)");
    println!("3. Install VS Code & Extensions (.NET Tool, MAUI, C# Dev Kit, Vue) - (OPTIONAL)");
    print!("Please select an option (1, 2, or 3): ");
    io::stdout().flush().unwrap();

    let mut choice = String::new();
    io::stdin().read_line(&mut choice).unwrap();
    let choice_trimmed = choice.trim();

    match choice_trimmed {
        "1" => {
            match os {
                "linux" => install_linux(arch, false),
                "windows" => install_windows(arch, false),
                "macos" => install_macos(arch, false),
                _ => eprintln!("Unsupported operating system: {}", os),
            }
        }
        "2" => {
            match os {
                "linux" => install_linux(arch, true),
                "windows" => install_windows(arch, true),
                "macos" => install_macos(arch, true),
                _ => eprintln!("Unsupported operating system: {}", os),
            }
        }
        "3" => {
            install_vscode_and_extensions(os);
        }
        _ => {
            eprintln!("Invalid option selected.");
        }
    }

    println!("\nInstallation process completed successfully!");
}

fn install_linux(arch: &str, include_maui: bool) {
    let os_release = fs::read_to_string("/etc/os-release").unwrap_or_default();

    let is_debian_family = os_release.contains("ID=debian") 
        || os_release.contains("ID=ubuntu") 
        || os_release.contains("ID=linuxmint") 
        || os_release.contains("ID_LIKE=debian")
        || os_release.contains("ID_LIKE=ubuntu");

    let is_fedora_rhel_family = os_release.contains("ID=fedora") 
        || os_release.contains("ID=rhel") 
        || os_release.contains("ID=centos") 
        || os_release.contains("ID=rocky") 
        || os_release.contains("ID=almalinux")
        || os_release.contains("ID_LIKE=fedora") 
        || os_release.contains("ID_LIKE=rhel")
        || Path::new("/etc/fedora-release").exists() 
        || Path::new("/etc/redhat-release").exists();

    let is_arch_family = os_release.contains("ID=arch") 
        || os_release.contains("ID_LIKE=arch");

    let is_suse_family = os_release.contains("ID=opensuse") 
        || os_release.contains("ID=sles") 
        || os_release.contains("ID_LIKE=suse");

    if is_fedora_rhel_family {
        println!("[Fedora / RHEL / Enterprise] Installing system dependencies...");
        let _ = Command::new("sudo").args(["dnf", "install", "-y", "zlib-ng-compat-devel", "gcc", "fuse", "make", "curl", "tar", "openssl-devel", "pkgconfig"]).status();
    } else if is_debian_family {
        println!("[Debian / Ubuntu / Mint] Installing system dependencies...");
        let _ = Command::new("sudo").args(["apt-get", "update"]).status();
        let _ = Command::new("sudo").args(["apt-get", "install", "-y", "build-essential", "zlib1g-dev", "libfuse2", "curl", "tar", "wget", "libssl-dev", "pkg-config"]).status();
    } else if is_arch_family {
        println!("[Arch Linux / Manjaro] Installing system dependencies...");
        let _ = Command::new("sudo").args(["pacman", "-Syu", "--noconfirm", "base-devel", "zlib", "fuse2", "curl", "tar", "openssl", "pkgconf"]).status();
    } else if is_suse_family {
        println!("[openSUSE] Installing system dependencies...");
        let _ = Command::new("sudo").args(["zypper", "install", "-y", "patterns-base-devel_basis", "zlib-devel", "fuse2", "curl", "tar", "libopenssl-devel", "pkg-config"]).status();
    } else {
        println!("[Warning] Unsupported Linux distribution. Skipping system dependencies installation...");
    }

    install_toolchain("linux", arch, include_maui);
}

fn install_windows(arch: &str, include_maui: bool) {
    let _ = Command::new("winget").args(["install", "-e", "--id", "Git.Git"]).status();
    
    let mut override_args = String::from("--quiet --add Microsoft.VisualStudio.Workload.VCTools --includeRecommended");
    if arch.contains("aarch64") || arch.contains("arm64") {
        override_args.push_str(" --add Microsoft.VisualStudio.Component.VC.Tools.ARM64");
    }

    let status_2026 = Command::new("winget")
        .args([
            "install", "-e", "--id", "Microsoft.VisualStudio.2026.BuildTools",
            "--override", &override_args
        ])
        .status();

    let mut success = match status_2026 {
        Ok(status) => status.success(),
        Err(_) => false,
    };

    if success {
        println!("[Windows] Microsoft.VisualStudio.2026.BuildTools was successfully downloaded and installed.");
    } else {
        let status_generic = Command::new("winget")
            .args([
                "install", "-e", "--id", "Microsoft.VisualStudio.BuildTools",
                "--override", &override_args
            ])
            .status();

        success = match status_generic {
            Ok(status) => status.success(),
            Err(_) => false,
        };

        if success {
            println!("[Windows] Microsoft.VisualStudio.BuildTools was successfully downloaded and installed.");
        } else {
            println!("[Windows] Installation skipped or packages were already present.");
        }
    }

    install_toolchain("windows", arch, include_maui);
}

fn install_macos(arch: &str, include_maui: bool) {
    println!("[macOS] Ensuring Xcode Command Line Tools are present...");
    let _ = Command::new("xcode-select").arg("--install").status();
    install_toolchain("macos", arch, include_maui);
}

fn install_toolchain(os: &str, arch: &str, include_maui: bool) {
    let rust_host_triple = match (os, arch) {
        ("linux", "x86_64") => "x86_64-unknown-linux-gnu",
        ("linux", "aarch64") => "aarch64-unknown-linux-gnu",
        ("windows", "x86_64") => "x86_64-pc-windows-msvc",
        ("windows", "aarch64") => "aarch64-pc-windows-msvc",
        ("macos", "x86_64") => "x86_64-apple-darwin",
        ("macos", "aarch64") => "aarch64-apple-darwin",
        _ => "x86_64-unknown-linux-gnu",
    };

    if Command::new("rustup").arg("--version").status().is_err() {
        println!("Installing Rust via rustup for target {}...", rust_host_triple);
        if os == "windows" {
            let ps_command = format!(
                "Invoke-WebRequest -UseBasicParsing 'https://win.rustup.rs' -OutFile 'rustup-init.exe'; ./rustup-init.exe -y --default-host {}; Remove-Item ./rustup-init.exe",
                rust_host_triple
            );
            let _ = Command::new("powershell")
                .args(["-Command", &ps_command])
                .status();
        } else {
            let sh_command = format!(
                "curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh -s -- -y --default-host {}",
                rust_host_triple
            );
            let _ = Command::new("sh")
                .args(["-c", &sh_command])
                .status();
            let _ = Command::new("sh")
                .args(["-c", "echo 'source \"$HOME/.cargo/env\"' >> ~/.bashrc"])
                .status();
        }
    } else {
        println!("Rust is already installed. Ensuring target {} is added...", rust_host_triple);
        let _ = Command::new("rustup")
            .args(["target", "add", rust_host_triple])
            .status();
    }

    let extra_targets = match (os, arch) {
        ("linux", "x86_64") => vec!["aarch64-unknown-linux-gnu"],
        ("linux", "aarch64") => vec!["x86_64-unknown-linux-gnu"],
        ("windows", "x86_64") => vec!["aarch64-pc-windows-msvc"],
        ("windows", "aarch64") => vec!["x86_64-pc-windows-msvc"],
        ("macos", "x86_64") => vec!["aarch64-apple-darwin"],
        ("macos", "aarch64") => vec!["x86_64-apple-darwin"],
        _ => vec![],
    };

    for target in extra_targets {
        println!("Ensuring target {} is added...", target);
        let _ = Command::new("rustup")
            .args(["target", "add", target])
            .status();
    }

    if Command::new("node").arg("--version").status().is_err() {
        println!("Installing Node.js v25.5.0 for {} ({}) ...", os, arch);
        
        let node_arch = match arch {
            "x86_64" => "x64",
            "aarch64" => "arm64",
            _ => "x64",
        };

        if os == "linux" || os == "macos" {
            let os_name = if os == "linux" { "linux" } else { "darwin" };
            let ext = if os == "linux" { "tar.xz" } else { "tar.gz" };
            let node_url = format!(
                "https://nodejs.org/dist/v25.5.0/node-v25.5.0-{}-{}.{}",
                os_name, node_arch, ext
            );
            
            println!("Downloading Node.js from: {}", node_url);
            let _ = Command::new("curl").args(["-o", "node_pkg.tar.xz", &node_url]).status();            
            let _ = Command::new("tar").args(["-xf", "node_pkg.tar.xz"]).status();
            let _ = fs::remove_file("node_pkg.tar.xz");
            println!("Node.js v25.5.0 extracted. Ensure it is added to your PATH.");
        } else if os == "windows" {
            let _ = Command::new("winget")
                .args(["install", "-e", "--id", "OpenJS.NodeJS.LTS", "--version", "25.5.0"])
                .status();
        }
    } else {
        println!("Node.js is already installed. Skipping.");
    }

    if Command::new("dotnet").arg("--version").status().is_err() {
        println!("Installing .NET 10 (v10.0.402)");
        if os == "windows" {
            let _ = Command::new("powershell")
                .args(["-Command", "& { Invoke-WebRequest -UseBasicParsing 'https://dot.net/v1/dotnet-install.ps1' -OutFile 'dotnet-install.ps1'; ./dotnet-install.ps1 -Version 10.0.402; Remove-Item ./dotnet-install.ps1 }"])
                .status();
        } else {
            let _ = Command::new("sh")
                .args(["-c", "curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 10.0.402"])
                .status();
        }
    } else {
        println!(".NET is already installed. Skipping.");
    }

    if include_maui {
        println!("Installing .NET MAUI workload...");
        let _ = Command::new("dotnet").args(["workload", "install", "maui"]).status();
    }

    println!("Installing .NET tool nuget-license...");
    let _ = Command::new("dotnet").args(["tool", "install", "--global", "nuget-license"]).status();

    if os == "linux" || os == "macos" {
        let home = std::env::var("HOME").unwrap_or_default();
        let tool_path = format!("{}/.dotnet/tools/nuget-license", home);
        let _ = Command::new("sudo").args(["ln", "-sf", &tool_path, "/usr/local/bin/nuget-license"]).status();
    } else if os == "windows" {
        let ps_script = "$path = [Environment]::GetEnvironmentVariable('PATH', 'User'); $toolPath = \"$env:USERPROFILE\\.dotnet\\tools\"; if ($path -notlike \"*$toolPath*\") { [Environment]::SetEnvironmentVariable('PATH', \"$path;$toolPath\", 'User') }";
        let _ = Command::new("powershell").args(["-Command", ps_script]).status();
    }
}

fn install_vscode_and_extensions(os: &str) {
    println!("\n[VS Code] Checking Visual Studio Code installation...");
    
    let code_installed = match os {
        "windows" => {
            Command::new("winget")
                .args(["list", "--id", "Microsoft.VisualStudioCode"])
                .status()
                .map(|s| s.success())
                .unwrap_or(false)
                || Command::new("code").arg("--version").status().is_ok()
        }
        "macos" => {
            Command::new("code").arg("--version").status().is_ok()
                || Path::new("/Applications/Visual Studio Code.app").exists()
        }
        _ => {
            Command::new("code").arg("--version").status().is_ok()
        }
    };

    if !code_installed {
        println!("[VS Code] Installing Visual Studio Code...");
        match os {
            "windows" => {
                let _ = Command::new("winget")
                    .args(["install", "-e", "--id", "Microsoft.VisualStudioCode"])
                    .status();
            }
            "macos" => {
                let _ = Command::new("brew")
                    .args(["install", "--cask", "visual-studio-code"])
                    .status();
            }
            "linux" => {
                let _ = Command::new("sudo").args(["apt-get", "install", "-y", "wget", "gpg"]).status();
                let _ = Command::new("sh").args(["-c", "wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > packages.microsoft.gpg"]).status();
                let _ = Command::new("sudo").args(["install", "-D", "-o", "root", "-g", "root", "-m", "644", "packages.microsoft.gpg", "/etc/apt/keyrings/packages.microsoft.gpg"]).status();
                let _ = Command::new("sh").args(["-c", "echo \"deb [arch=amd64,arm64,armhf signed-by=/etc/apt/keyrings/packages.microsoft.gpg] https://packages.microsoft.com/repos/code stable main\" | sudo tee /etc/apt/sources.list.d/vscode.list > /dev/null"]).status();
                let _ = Command::new("rm").args(["packages.microsoft.gpg"]).status();
                let _ = Command::new("sudo").args(["apt-get", "update"]).status();
                let _ = Command::new("sudo").args(["apt-get", "install", "-y", "code"]).status();
            }
            _ => {}
        }
    } else {
        println!("[VS Code] Visual Studio Code is already installed. Skipping.");
    }

    let extensions = vec![
        "ms-dotnettools.vscode-dotnet-runtime",
        "ms-dotnettools.dotnet-maui",
        "ms-dotnettools.csdevkit",
        "Vue.volar",
    ];

    println!("[VS Code] Checking and installing required extensions...");
    
    let output = Command::new("code")
        .arg("--list-extensions")
        .output();

    let installed_extensions = match output {
        Ok(out) => String::from_utf8_lossy(&out.stdout).to_lowercase(),
        Err(_) => String::new(),
    };

    for ext in extensions {
        if installed_extensions.contains(&ext.to_lowercase()) {
            println!("  -> Extension '{}' is already installed. Skipping.", ext);
        } else {
            println!("  -> Installing extension '{}'...", ext);
            let _ = Command::new("code")
                .args(["--install-extension", ext])
                .status();
        }
    }
}