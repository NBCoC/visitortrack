import { customAttribute, ComponentAttached, ComponentDetached } from 'aurelia-framework';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';
import { LoadingEvent } from '../core/models';

@customAttribute('vt-loading-form-indicator')
export class LoadingFormIndicatorCustomAttribute implements ComponentAttached, ComponentDetached {
  private element: Element;
  private eventAggregator: EventAggregator;
  private subscription: Subscription;

  constructor(element: Element, eventAggregator: EventAggregator) {
    this.element = element;
    this.eventAggregator = eventAggregator;
  }

  public attached(): void {
    this.subscription = this.eventAggregator.subscribe(LoadingEvent, (e: LoadingEvent) => {
      this.toggleInputs(e.isLoading);
      this.toggleSelect(e.isLoading);
      this.toggleSubmitButton(e.isLoading);
    });
  }

  public detached(): void {
    if (!this.subscription) return;
    this.subscription.dispose();
  }

  private toggleInputs(isLoading: boolean): void {
    const elements = this.element.querySelectorAll('input');
    if (!elements) return;
    let element;
    for (let index = 0; index < elements.length; index++) {
      element = elements[index];
      if (isLoading) {
        element.setAttribute('disabled', '');
      } else {
        element.removeAttribute('disabled');
      }
    }
  }

  private toggleSelect(isLoading: boolean) {
    const elements = this.element.querySelectorAll('select');
    if (!elements) return;
    let element;
    for (let index = 0; index < elements.length; index++) {
      element = elements[index];
      if (isLoading) {
        element.setAttribute('disabled', '');
      } else {
        element.removeAttribute('disabled');
      }
    }
  }

  private toggleSubmitButton(isLoading: boolean): void {
    const elements = this.element.querySelectorAll('button');
    if (!elements) return;
    let element;
    for (let index = 0; index < elements.length; index++) {
      element = elements[index];
      if (isLoading) {
        element.setAttribute('disabled', '');
      } else {
        element.removeAttribute('disabled');
      }
    }
  }
}
