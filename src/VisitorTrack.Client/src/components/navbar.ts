import { bindable } from 'aurelia-framework';
import { User } from '../core/models';

export class NavbarCustomElement {
  @bindable() public user: User;
  @bindable() public signOut: Function;

  constructor() {
    this.user = {} as User;
  }

  public onSignOut(): void {
    if (!this.signOut) return;
    this.signOut();
  }
}
