import { Aurelia, PLATFORM } from 'aurelia-framework';
import * as Bluebird from 'bluebird';

Bluebird.config({ warnings: false });

export async function configure(aurelia: Aurelia) {
  aurelia.use
    .standardConfiguration()
    .feature(PLATFORM.moduleName('components/index'))
    .feature(PLATFORM.moduleName('attributes/index'))
    .feature(PLATFORM.moduleName('converters/index'))
    .plugin(PLATFORM.moduleName('aurelia-validation'))
    .plugin(PLATFORM.moduleName('aurelia-dialog'), cfg => cfg.useDefaults());

  if (DEV_MODE) {
    aurelia.use.developmentLogging();
  }

  await aurelia.start();
  await aurelia.setRoot(PLATFORM.moduleName('app'));
}
