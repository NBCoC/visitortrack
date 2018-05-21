import { inlineView, customElement, ComponentAttached, bindable, inject, DOM, bindingMode } from 'aurelia-framework';
import * as Pikaday from 'pikaday';

@inject(DOM.Element)
@customElement('vt-date-picker')
@inlineView(
  `<template>
    <input type="text" class.bind="classList" readonly />
  </template>`
)
export class DatePickerCustomElement implements ComponentAttached {
  private element: Element;
  @bindable classList: string;

  @bindable({ defaultBindingMode: bindingMode.twoWay })
  value: Date;

  constructor(element: Element) {
    this.element = element;
  }

  public attached(): void {
    const datePicker = new Pikaday({
      format: 'MM/DD/YYYY',
      field: this.element.querySelector('input'),
      onSelect: () => (this.value = datePicker.getMoment().toDate())
    });

    if (!this.value) return;

    datePicker.setDate(this.value);
  }
}
