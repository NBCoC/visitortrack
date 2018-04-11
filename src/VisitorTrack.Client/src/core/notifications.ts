import 'toastr/build/toastr.css';
import * as toastr from 'toastr';

toastr.options = {
  closeButton: false,
  debug: false,
  newestOnTop: false,
  progressBar: false,
  positionClass: 'toast-bottom-full-width',
  preventDuplicates: true,
  onclick: null,
  showDuration: '300',
  hideDuration: '1000',
  timeOut: '5000',
  extendedTimeOut: '1000',
  showEasing: 'swing',
  hideEasing: 'linear',
  showMethod: 'fadeIn',
  hideMethod: 'fadeOut'
};

export const notifyError = (message: string): void => toastr.error(message);
export const notifySuccess = (message: string): void => toastr.success(message);
export const notifyWarning = (message: string): void => toastr.warning(message);
export const notifyInfo = (message: string): void => toastr.info(message);
