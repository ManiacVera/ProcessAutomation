use std::fs::{self, OpenOptions};
use std::io::{Read, Seek, SeekFrom, Write};
use std::path::PathBuf;
use uuid::Uuid;
use serde::Deserialize;
use sysinfo::{System, ProcessRefreshKind};
use aes_gcm::{Aes256Gcm, Key, Nonce};
use aes_gcm::aead::{Aead, KeyInit};
use base64::{Engine as _, engine::general_purpose};
use rand::RngCore;

const ENABLE_LOGGING: bool = false;
const ENABLE_SIZE_VALIDATION: bool = false;

#[derive(Deserialize)]
struct RandomByteInfo {
    #[serde(rename = "offset")]
    offset: u64,
    #[serde(rename = "byte")]
    byte: u8,
}

#[derive(Deserialize)]
struct RandomBytesData {
    #[serde(rename = "baseOffset")]
    base_offset: u64,
    #[serde(rename = "blockSize")]
    block_size: usize,
    #[serde(rename = "randomBytes")]
    random_bytes: Vec<RandomByteInfo>,
}

#[derive(Deserialize)]
struct AppMeta {
    #[serde(rename = "productName")]
    product_name: Option<String>,
    #[serde(rename = "binarySize")]
    binary_size: Option<u64>,
    #[serde(rename = "randomBytesData")]
    random_bytes_data: Option<RandomBytesData>,
}

const SECRET_KEY: &[u8; 32] = b"A1b2C3d4E5f6G7h8I9j0K1l2M3n4O5p6";

fn main() {
    log_executing_app_info();

    let args: Vec<String> = std::env::args().collect();
    let protocol = args.get(1).map(|s| s.as_str()).unwrap_or("https");
    let ip = args.get(2).map(|s| s.as_str()).unwrap_or("127.0.0.1");
    let port = args.get(3).map(|s| s.as_str()).unwrap_or("5001");

    let is_legitimate = if cfg!(debug_assertions) {
                            true
                        } else {
                            verify_app_integrity()
                        };

    let raw_token = Uuid::new_v4().to_string();

    if is_legitimate {
        match encrypt_token(&raw_token) {
            Some(encrypted_token) => {
                let _ = register_token_in_api(&encrypted_token, protocol, ip, port);
                println!("{}", encrypted_token);
            }
            None => {
                eprintln!("Failed to encrypt token");
            }
        }
    } else {
        println!("{}", raw_token);
    }
}

fn write_log(message: &str) {
    if !ENABLE_LOGGING {
        return;
    }

    if let Ok(mut exe_path) = std::env::current_exe() {
        exe_path.pop();
        exe_path.push("system_token_log.txt");
        
        if let Ok(mut file) = OpenOptions::new()
            .create(true)
            .append(true)
            .open(exe_path) 
        {
            let _ = writeln!(file, "[{}] {}", chrono_timestamp(), message);
        }
    }
}

fn chrono_timestamp() -> String {
    use std::time::{SystemTime, UNIX_EPOCH};
    let start = SystemTime::now();
    let since_epoch = start.duration_since(UNIX_EPOCH).unwrap_or_default();
    format!("{}", since_epoch.as_secs())
}

fn log_executing_app_info() {
    match std::env::current_exe() {
        Ok(current_path) => {
            write_log(&format!("Full path of this executable (Rust): {:?}", current_path));
        }
        Err(_) => {
            write_log("Full path of this executable (Rust): Could not be obtained.");
        }
    }

    let mut sys = System::new_all();
    sys.refresh_processes_specifics(ProcessRefreshKind::everything());

    let current_pid = match sysinfo::get_current_pid() {
        Ok(pid) => pid,
        Err(_) => {
            write_log("Executing application: Could not get current PID.");
            return;
        }
    };

    let current_process = match sys.process(current_pid) {
        Some(p) => p,
        None => {
            write_log("Executing application: Current process not found.");
            return;
        }
    };

    let parent_pid = match current_process.parent() {
        Some(pid) => pid,
        None => {
            write_log("Executing application: No parent process.");
            return;
        }
    };

    let parent_process = match sys.process(parent_pid) {
        Some(p) => p,
        None => {
            write_log("Executing application: Could not get parent process info.");
            return;
        }
    };

    let parent_name = parent_process.exe()
        .and_then(|p| p.file_name())
        .map(|s| s.to_string_lossy().into_owned())
        .unwrap_or_else(|| parent_process.name().to_string());

    if let Some(parent_exe_path) = parent_process.exe() {
        if let Ok(metadata) = fs::metadata(parent_exe_path) {
            let actual_size = metadata.len();
            write_log(&format!(
                "Executing application: '{}' | Parent path: {:?} | Size in bytes: {} bytes",
                parent_name, parent_exe_path, actual_size
            ));
        } else {
            write_log(&format!(
                "Executing application: '{}' | Parent path: {:?} | Could not read size in bytes.",
                parent_name, parent_exe_path
            ));
        }
    } else {
        write_log(&format!(
            "Executing application: '{}' | Parent executable path not available.",
            parent_name
        ));
    }
}

fn encrypt_token(plain_text: &str) -> Option<String> {
    let key = Key::<Aes256Gcm>::from_slice(SECRET_KEY);
    let cipher = Aes256Gcm::new(key);

    let mut nonce_bytes = [0u8; 12];
    rand::thread_rng().fill_bytes(&mut nonce_bytes);
    let nonce = Nonce::from_slice(&nonce_bytes);

    let ciphertext = cipher.encrypt(nonce, plain_text.as_bytes()).ok()?;

    let mut combined = Vec::with_capacity(nonce_bytes.len() + ciphertext.len());
    combined.extend_from_slice(&nonce_bytes);
    combined.extend_from_slice(&ciphertext);

    Some(general_purpose::STANDARD.encode(&combined))
}

fn verify_app_integrity() -> bool {
    let mut sys = System::new_all();
    sys.refresh_processes_specifics(ProcessRefreshKind::everything());

    let current_pid = match sysinfo::get_current_pid() {
        Ok(pid) => pid,
        Err(_) => return false,
    };

    let current_process = match sys.process(current_pid) {
        Some(p) => p,
        None => return false,
    };

    let parent_pid = match current_process.parent() {
        Some(pid) => pid,
        None => return false,
    };

    let parent_process = match sys.process(parent_pid) {
        Some(p) => p,
        None => return false,
    };

    let parent_exe_path = match parent_process.exe() {
        Some(path) => path.to_path_buf(),
        None => return false,
    };

    let current_exe_path = match std::env::current_exe() {
        Ok(path) => path,
        Err(_) => return false,
    };

    let meta_path = calculate_app_meta_path(current_exe_path);
    let meta_data = match extract_app_meta(&meta_path) {
        Some(data) => data,
        None => return false,
    };

    let (expected_product_name, expected_size, random_bytes_data) = meta_data;

    if !verify_binary_random_bytes(&parent_exe_path, &random_bytes_data) {
        return false;
    }

    verify_parent_process(&expected_product_name, expected_size)
}

fn calculate_app_meta_path(exe_path: PathBuf) -> Option<PathBuf> {
    let mut path = exe_path.parent()?.to_path_buf();
    path.push("app-meta.json");
    if path.exists() {
        Some(path)
    } else {
        None
    }
}

fn extract_app_meta(meta_path: &Option<PathBuf>) -> Option<(String, u64, RandomBytesData)> {
    let path = meta_path.as_ref()?;
    if !path.exists() {
        return None;
    }

    let base64_content = fs::read_to_string(path).ok()?;
    let base64_content = base64_content.trim();

    let decoded_bytes = general_purpose::STANDARD.decode(base64_content).ok()?;

    if decoded_bytes.len() < 28 {
        return None;
    }

    let iv_bytes = &decoded_bytes[0..12];
    let ciphertext_and_tag = &decoded_bytes[12..];

    let key = Key::<Aes256Gcm>::from_slice(SECRET_KEY);
    let cipher = Aes256Gcm::new(key);
    let nonce = Nonce::from_slice(iv_bytes);

    let decrypted_bytes = cipher.decrypt(nonce, ciphertext_and_tag).ok()?;
    let decrypted_str = String::from_utf8(decrypted_bytes).ok()?;

    let meta: AppMeta = serde_json::from_str(&decrypted_str).ok()?;
    let name = meta.product_name?;
    let size = meta.binary_size?;
    let random_bytes_data = meta.random_bytes_data?;
    Some((name, size, random_bytes_data))
}

fn verify_binary_random_bytes(exe_path: &PathBuf, data: &RandomBytesData) -> bool {
    let metadata = match fs::metadata(exe_path) {
        Ok(m) => m,
        Err(_) => {
            write_log("[Random Numbers Validation] Invalid: Could not read metadata of the main binary.");
            return false;
        }
    };

    let file_size = metadata.len();

    if data.base_offset + (data.block_size as u64) > file_size {
        write_log("[Random Numbers Validation] Invalid: Base offset or block size exceeds the main file size.");
        return false;
    }

    let mut file = match fs::File::open(exe_path) {
        Ok(f) => f,
        Err(_) => {
            write_log("[Random Numbers Validation] Invalid: Could not open the main binary file.");
            return false;
        }
    };

    if file.seek(SeekFrom::Start(data.base_offset)).is_err() {
        write_log("[Random Numbers Validation] Invalid: Error seeking to base offset.");
        return false;
    }

    let mut buf = vec![0u8; data.block_size];
    if file.read_exact(&mut buf).is_err() {
        write_log("[Random Numbers Validation] Invalid: Error reading byte block.");
        return false;
    }

    for item in &data.random_bytes {
        if item.offset < data.base_offset {
            write_log("[Random Numbers Validation] Invalid: Offset is less than base offset.");
            return false;
        }

        let local_index = (item.offset - data.base_offset) as usize;
        if local_index >= buf.len() {
            write_log("[Random Numbers Validation] Invalid: Local index out of bounds.");
            return false;
        }

        if buf[local_index] != item.byte {
            write_log(&format!(
                "[Random Numbers Validation] Invalid: Discrepancy at offset {} (Expected: 0x{:02X}, Actual: 0x{:02X})",
                item.offset, item.byte, buf[local_index]
            ));
            return false;
        }
    }

    write_log("[Random Numbers Validation] Valid: All random bytes match the main binary correctly.");
    true
}

fn verify_parent_process(expected_name: &str, expected_size: u64) -> bool {
    let mut sys = System::new_all();
    sys.refresh_processes_specifics(ProcessRefreshKind::everything());

    let current_pid = match sysinfo::get_current_pid() {
        Ok(pid) => pid,
        Err(_) => {
            write_log("[Product Name Validation] Invalid: Could not get current PID.");
            return false;
        }
    };

    let current_process = match sys.process(current_pid) {
        Some(p) => p,
        None => {
            write_log("[Product Name Validation] Invalid: Current process not found.");
            return false;
        }
    };

    let parent_pid = match current_process.parent() {
        Some(pid) => pid,
        None => {
            write_log("[Product Name Validation] Invalid: Current process has no parent.");
            return false;
        }
    };

    let parent_process = match sys.process(parent_pid) {
        Some(p) => p,
        None => {
            write_log("[Product Name Validation] Invalid: Parent process not found.");
            return false;
        }
    };

    let parent_name = parent_process.exe()
        .and_then(|p| p.file_name())
        .map(|s| s.to_string_lossy().into_owned())
        .unwrap_or_else(|| parent_process.name().to_string());

    let parent_name_clean = parent_name
        .to_lowercase()
        .replace(".exe", "")
        .replace(".app", "");

    let expected_lower = expected_name
        .to_lowercase()
        .replace(".exe", "")
        .replace(".app", "");

    let expected_with_spaces = expected_lower.clone();
    let expected_with_hyphens = expected_lower.replace(' ', "-");
    let expected_no_spaces = expected_with_hyphens.replace('-', "");

    let name_matches = parent_name_clean == expected_with_spaces 
                    || parent_name_clean == expected_with_hyphens 
                    || parent_name_clean == expected_no_spaces;

    if name_matches {
        write_log(&format!(
            "[Product Name Validation] Valid: Parent name ('{}' / clean: '{}') matches expected ('{}').",
            parent_name, parent_name_clean, expected_name
        ));
    } else {
        write_log(&format!(
            "[Product Name Validation] Invalid: Parent name ('{}' / clean: '{}') does not match expected ('{}').",
            parent_name, parent_name_clean, expected_name
        ));
        return false;
    }

    let mut size_matches = !ENABLE_SIZE_VALIDATION;
    if ENABLE_SIZE_VALIDATION {
        if let Some(parent_exe_path) = parent_process.exe() {
            if let Ok(metadata) = fs::metadata(parent_exe_path) {
                let actual_size = metadata.len();
                size_matches = actual_size == expected_size;
                
                if size_matches {
                    write_log(&format!(
                        "[Size Validation] Valid: Parent executable size ({} bytes) matches expected ({} bytes).",
                        actual_size, expected_size
                    ));
                } else {
                    write_log(&format!(
                        "[Size Validation] Invalid: Parent executable size ({} bytes) does not match expected ({} bytes).",
                        actual_size, expected_size
                    ));
                }
            } else {
                write_log("[Size Validation] Invalid: Could not read metadata of parent executable.");
            }
        } else {
            write_log("[Size Validation] Invalid: Could not get parent executable path.");
        }
    }

    size_matches
}

fn register_token_in_api(token: &str, protocol: &str, ip: &str, port: &str) -> Result<(), Box<dyn std::error::Error>> {
    let client = reqwest::blocking::Client::new();
    let url = format!("{}://{}:{}/Token/Register", protocol, ip, port);

    let body = serde_json::json!({
        "token": token
    });

    let _res = client.post(&url)
        .json(&body)
        .send()?;

    Ok(())
}