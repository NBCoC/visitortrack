import { inlineView, ComponentAttached, ComponentDetached, DOM, inject, customElement } from 'aurelia-framework';
import { LoadingEvent } from './../core/models';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';

@inject(DOM.Element, EventAggregator)
@inlineView('<template><div class="loader u-hide"></div></template>')
@customElement('vt-loader')
export class LoaderCustomElement implements ComponentAttached, ComponentDetached {
  private element: Element;
  private eventAggregator: EventAggregator;
  private subscription: Subscription;

  constructor(element: Element, eventAggregator: EventAggregator) {
    this.element = element;
    this.eventAggregator = eventAggregator;
  }

  public attached(): void {
    this.subscription = this.eventAggregator.subscribe(LoadingEvent, (e: LoadingEvent) => {
      const element = this.element.querySelector('.loader');
      if (!element) return;

      if (e.isLoading) {
        element.classList.add('u-show');
        element.classList.remove('u-hide');
      } else {
        element.classList.add('u-hide');
        element.classList.remove('u-show');
      }
    });
  }

  public detached(): void {
    if (!this.subscription) return;
    this.subscription.dispose();
  }
}
