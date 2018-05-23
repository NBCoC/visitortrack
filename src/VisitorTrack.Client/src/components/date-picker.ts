import { inlineView, customElement, ComponentAttached, bindable, inject, DOM, bindingMode } from 'aurelia-framework';
import * as Pikaday from 'pikaday';

@inject(DOM.Element)
@customElement('vt-date-picker')
@inlineView(
  `<template>
    <div class="vt-date-picker">
      <input type="text" class.bind="classList" readonly />
      <button type="button" class="button-danger" click.trigger="clearValue()">X</button>
    </div>
  </template>`
)
export class DatePickerCustomElement implements ComponentAttached {
  private element: Element;
  private datePicker: any;
  @bindable classList: string;

  @bindable({ defaultBindingMode: bindingMode.twoWay })
  value: Date;

  constructor(element: Element) {
    this.element = element;
  }

  public attached(): void {
    this.datePicker = new Pikaday({
      format: 'MM/DD/YYYY',
      field: this.element.querySelector('input'),
      onSelect: () => (this.value = this.datePicker.getMoment().toDate())
    });

    if (!this.value) return;

    this.datePicker.setDate(this.value);
  }

  public clearValue(): void {
    this.value = null;
    this.datePicker.setDate(null);
  }
}
