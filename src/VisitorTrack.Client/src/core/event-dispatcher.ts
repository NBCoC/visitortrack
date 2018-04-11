export class EventDispatcher {
  public static dispatch(element: Element, eventName: string, args?: any): boolean {
    let customEvent: CustomEvent;

    if ((window as any).CustomEvent) {
      customEvent = new CustomEvent(eventName, {
        detail: { args: args },
        bubbles: true
      });
    } else {
      customEvent = document.createEvent('CustomEvent');

      customEvent.initCustomEvent(eventName, true, true, {
        detail: { args: args }
      });
    }

    element.dispatchEvent(customEvent);

    return true;
  }
}
