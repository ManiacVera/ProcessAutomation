import { toast } from 'vue3-toastify'

const DEFAULT_TITLE = "Process Automation"

const formatMessage = (message, title) => {
  const header = title || DEFAULT_TITLE
  return header ? `${header}: ${message}` : message
}

export const showWarningToast = (message, title) => {
  toast.warning(formatMessage(message, title))
}

export const showErrorToast = (message, title) => {
  toast.error(formatMessage(message, title))
}

export const showSuccessToast = (message, title) => {
  toast.info(formatMessage(message, title))
}