import { IsLoadingEvent } from './../../core/models';
import { customAttribute } from 'aurelia-framework';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';

@customAttribute('visitor-track-loading-indicator')
export class LoadingIndicatorAttribute {
  private element: Element;
  private eventAggregator: EventAggregator;
  private subscription: Subscription;

  constructor(element: Element, eventAggregator: EventAggregator) {
    this.element = element;
    this.eventAggregator = eventAggregator;
  }

  public attached(): void {
    this.subscription = this.eventAggregator.subscribe(
      IsLoadingEvent,
      (e: IsLoadingEvent) => {
        if (e.args) {
          this.element.classList.add('is-loading');
          this.element.setAttribute('disabled', '');
        } else {
          this.element.classList.remove('is-loading');
          this.element.removeAttribute('disabled');
        }
      }
    );
  }

  public detached(): void {
    if (!this.subscription) return;
    this.subscription.dispose();
  }
}
