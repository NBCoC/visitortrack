//import 'bulma-calendar/bulma-calendar';
import { Aurelia, PLATFORM } from 'aurelia-framework';
import * as Bluebird from 'bluebird';

Bluebird.config({ warnings: false });

export async function configure(aurelia: Aurelia) {
  aurelia.use
    .standardConfiguration()
    .developmentLogging()
    .feature(PLATFORM.moduleName('components/index'))
    .feature(PLATFORM.moduleName('resources/index'))
    .plugin(PLATFORM.moduleName('aurelia-validation'))
    .plugin(PLATFORM.moduleName('aurelia-dialog'), cfg => cfg.useDefaults());

  await aurelia.start();
  await aurelia.setRoot(PLATFORM.moduleName('app'));
}
