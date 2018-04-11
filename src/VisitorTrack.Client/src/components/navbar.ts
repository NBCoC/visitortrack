import { User } from './../core/models';
import { customElement, bindable } from 'aurelia-framework';

@customElement('visitor-track-navbar')
export class NavbarCustomElement {
  private element: Element;
  @bindable() user: User;
  @bindable() signOut: Function;
  @bindable() changePassword: Function;

  constructor(element: Element) {
    this.element = element;
    this.user = {} as User;
  }

  public attached(): void {
    const element = this.element.querySelector('.navbar-burger');
    if (!element) return;
    const that = this;
    element.addEventListener('click', () => {
      element.classList.toggle('is-active');
      const targetId = (element as any).dataset.target;
      const target = that.element.querySelector(`#${targetId}`) as Element;
      if (!target) return;
      target.classList.toggle('is-active');
    });
  }

  public onSignOut(): void {
    if (!this.signOut) return;
    this.signOut();
  }

  public onChangePassword(): void {
    if (!this.changePassword) return;
    this.changePassword();
  }
}
