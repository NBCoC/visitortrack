import Vue from 'vue';

export const Bus = new Vue();
export const DialogEvent = 'display-dialog';

export const confirm = (title, message, callback) =>
  Bus.$emit(DialogEvent, {
    title: title,
    message: message,
    callback: callback,
    saveBtnText: 'Yes',
    saveBtnIcon: 'fa fa-thumbs-up',
    cancelBtnText: 'No',
    cancelBtnIcon: 'fa fa-thumbs-down'
  });

export const alert = (title, message) =>
  Bus.$emit(DialogEvent, {
    title: title,
    message: message,
    isAlert: true,
    saveBtnText: 'OK',
    saveBtnIcon: 'fa fa-thumbs-up'
  });

export const apiError = error =>
  alert(
    'Visitor-Track',
    error.response.status === 400
      ? error.response.data
      : 'API error occurred. Please contact System Administrator'
  );
