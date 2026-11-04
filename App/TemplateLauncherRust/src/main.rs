use std::path::Path;
use std::process::Command;

const MAGIC: [u8; 8] = *b"LCHRDATA";
const END_MAGIC: [u8; 8] = *b"LCHREND!";
const VERSION: u32 = 1;

const BINARY_PATH_SIZE: usize = 2048;
const ENCRYPTED_PAYLOAD_SIZE: usize = 2048;

#[repr(C)]
pub struct LauncherData {
    pub magic: [u8; 8],
    pub version: u32,
    pub size: u32,
    pub binary_path: [u8; BINARY_PATH_SIZE],
    pub encrypted_payload: [u8; ENCRYPTED_PAYLOAD_SIZE],
    pub end_magic: [u8; 8],
}

#[used]
#[unsafe(link_section = ".launcher")]
#[cfg(any(target_os = "windows", target_os = "linux"))]
static LAUNCHER_DATA: LauncherData = LauncherData {
    magic: MAGIC,
    version: VERSION,
    size: std::mem::size_of::<LauncherData>() as u32,
    binary_path: [0; BINARY_PATH_SIZE],
    encrypted_payload: [0; ENCRYPTED_PAYLOAD_SIZE],
    end_magic: END_MAGIC,
};

#[used]
#[unsafe(link_section = "__DATA,__launcher")]
#[cfg(target_os = "macos")]
static LAUNCHER_DATA: LauncherData = LauncherData {
    magic: MAGIC,
    version: VERSION,
    size: std::mem::size_of::<LauncherData>() as u32,
    binary_path: [0; BINARY_PATH_SIZE],
    encrypted_payload: [0; ENCRYPTED_PAYLOAD_SIZE],
    end_magic: END_MAGIC,
};

fn read_fixed_string(data: &[u8]) -> Result<String, Box<dyn std::error::Error>> {
    let length = data.iter().position(|&byte| byte == 0).unwrap_or(data.len());
    let value = std::str::from_utf8(&data[..length])?;
    Ok(value.to_string())
}

fn main() {
    if let Err(error) = run() {
        eprintln!("Launcher error: {}", error);
        std::process::exit(1);
    }
}

fn run() -> Result<(), Box<dyn std::error::Error>> {
    const EXPECTED_SIZE: usize = 8 + 4 + 4 + 2048 + 2048 + 8;

    if std::mem::size_of::<LauncherData>() != EXPECTED_SIZE {
        return Err(format!(
            "Invalid LauncherData size: {}. Expected {}.",
            std::mem::size_of::<LauncherData>(),
            EXPECTED_SIZE
        )
        .into());
    }

    let config = &LAUNCHER_DATA;

    if config.magic != MAGIC {
        return Err("Invalid launcher MAGIC.".into());
    }

    if config.version != VERSION {
        return Err(format!("Unsupported launcher version: {}.", config.version).into());
    }

    if config.size as usize != std::mem::size_of::<LauncherData>() {
        return Err("Invalid launcher data size.".into());
    }

    if config.end_magic != END_MAGIC {
        return Err("Invalid launcher END_MAGIC.".into());
    }

    let binary_path = read_fixed_string(&config.binary_path)?;
    let encrypted_payload = read_fixed_string(&config.encrypted_payload)?;

    if binary_path.is_empty() {
        return Err("binaryPath is empty.".into());
    }

    let mut cmd = Command::new(&binary_path);

    if let Some(parent_dir) = Path::new(&binary_path).parent() {
        if !parent_dir.as_os_str().is_empty() {
            cmd.current_dir(parent_dir);
        }
    }

    cmd.arg(&encrypted_payload);

    let output = cmd.output()?;
    let exit_code = output.status.code();
    let stdout = String::from_utf8_lossy(&output.stdout);
    let stderr = String::from_utf8_lossy(&output.stderr);

    if !stdout.is_empty() {
        println!("{}", stdout);
    }

    if !stderr.is_empty() {
        eprintln!("{}", stderr);
    }

    if !output.status.success() {
        return Err(format!("Process exited with code {:?}.", exit_code).into());
    }

    Ok(())
}