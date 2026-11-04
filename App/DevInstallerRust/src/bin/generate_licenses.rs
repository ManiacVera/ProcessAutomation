use std::fs;
use std::path::Path;
use cargo_license::{get_dependencies_from_cargo_lock, GetDependenciesOpt};
use cargo_metadata::MetadataCommand;

fn main() {
    let out_dir = Path::new("target/release/third-party-licenses");
    let licenses_dir = out_dir.join("licenses");
    
    fs::create_dir_all(&licenses_dir).unwrap();

    let mut notices = String::from("# THIRD-PARTY NOTICES\n\nThis software uses the following third-party libraries:\n\n");

    let metadata = MetadataCommand::new();
    let opts = GetDependenciesOpt::default();
    let deps = get_dependencies_from_cargo_lock(metadata, opts).unwrap();

    let cargo_home = std::env::var("CARGO_HOME")
        .unwrap_or_else(|_| format!("{}/.cargo", std::env::var("HOME").unwrap_or_default()));
    let registry_src = Path::new(&cargo_home).join("registry/src");

    for dep in deps {
        let name = dep.name;
        let version = dep.version;
        let license = dep.license.unwrap_or_else(|| "Unknown".to_string());
        
        let mut full_license_text = String::new();

        if registry_src.exists() {
            if let Ok(entries) = fs::read_dir(&registry_src) {
                'outer: for entry in entries.flatten() {
                    let folder = entry.path();
                    if folder.is_dir() {
                        let crate_folder = folder.join(format!("{}-{}", name, version));
                        if crate_folder.exists() && crate_folder.is_dir() {
                            if let Ok(files) = fs::read_dir(&crate_folder) {
                                for file in files.flatten() {
                                    let file_name = file.file_name();
                                    let name_upper = file_name.to_string_lossy().to_uppercase();
                                    if name_upper.starts_with("LICENSE") || name_upper.starts_with("LICENCE") || name_upper.starts_with("COPYING") {
                                        if let Ok(content) = fs::read_to_string(file.path()) {
                                            full_license_text.push_str(&format!("=== FILE: {} ===\n", file_name.to_string_lossy()));
                                            full_license_text.push_str(&content);
                                            full_license_text.push_str("\n\n\n");
                                        }
                                    }
                                }
                                if !full_license_text.is_empty() {
                                    break 'outer;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Si no hay archivo físico, proveemos el texto estándar según el tipo declarado
        if full_license_text.is_empty() {
            let upper_lic = license.to_uppercase();
            if upper_lic.contains("MIT") {
                full_license_text.push_str("MIT License\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.\n\n");
            }
            if upper_lic.contains("APACHE") {
                full_license_text.push_str("Apache License\nVersion 2.0, January 2004\nhttp://www.apache.org/licenses/\n\nTERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION\n\n...");
            }
            if full_license_text.is_empty() {
                full_license_text = format!("License: {}\n(Standard text not mapped, declared in Cargo.toml)", license);
            }
        }

        let file_name = format!("{}-{}.txt", name, version);
        let file_path = licenses_dir.join(&file_name);

        fs::write(&file_path, &full_license_text).unwrap();

        notices.push_str(&format!("## {} v{}\n", name, version));
        notices.push_str(&format!("- **License:** `{}`\n", license));
        notices.push_str(&format!("- **License text:** [`licenses/{}`](licenses/{})\n\n", file_name, file_name));
        notices.push_str("---\n\n");
    }

    let notices_path = out_dir.join("THIRD_PARTY_LICENSES_RUST.md");
    fs::write(&notices_path, notices).unwrap();
}