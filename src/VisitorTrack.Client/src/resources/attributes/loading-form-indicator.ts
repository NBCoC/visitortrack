import { IsLoadingEvent } from './../../core/models';
import { customAttribute } from 'aurelia-framework';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';

@customAttribute('visitor-track-loading-form-indicator')
export class LoadingFormIndicatorAttribute {
  private element: Element;
  private eventAggregator: EventAggregator;
  private subscription: Subscription;

  constructor(element: Element, eventAggregator: EventAggregator) {
    this.element = element;
    this.eventAggregator = eventAggregator;
  }

  public attached(): void {
    this.subscription = this.eventAggregator.subscribe(IsLoadingEvent, (e: IsLoadingEvent) => {
      this.toggleInputs(e.args);
      this.toggleSelect(e.args);
      this.toggleSubmitButton(e.args);
    });
  }

  public detached(): void {
    if (!this.subscription) return;
    this.subscription.dispose();
  }

  private toggleInputs(isLoading: boolean): void {
    const elements = this.element.querySelectorAll('input');
    if (!elements) return;
    for (let index = 0; index < elements.length; index++) {
      const element = elements[index];
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
    for (let index = 0; index < elements.length; index++) {
      const element = elements[index];
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
    for (let index = 0; index < elements.length; index++) {
      const element = elements[index];
      if (isLoading) {
        element.classList.add('is-loading');
        element.setAttribute('disabled', '');
      } else {
        element.classList.remove('is-loading');
        element.removeAttribute('disabled');
      }
    }
  }
}
