import { customAttribute, ComponentAttached, ComponentDetached } from 'aurelia-framework';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';
import { LoadingEvent } from '../core/models';

@customAttribute('vt-loading-indicator')
export class LoadingIndicatorCustomAttribute implements ComponentAttached, ComponentDetached {
  private element: Element;
  private eventAggregator: EventAggregator;
  private subscription: Subscription;

  constructor(element: Element, eventAggregator: EventAggregator) {
    this.element = element;
    this.eventAggregator = eventAggregator;
  }

  public attached(): void {
    this.subscription = this.eventAggregator.subscribe(LoadingEvent, (e: LoadingEvent) => {
      if (e.isLoading) {
        this.element.setAttribute('disabled', '');
      } else {
        this.element.removeAttribute('disabled');
      }
    });
  }

  public detached(): void {
    if (!this.subscription) return;
    this.subscription.dispose();
  }
}
