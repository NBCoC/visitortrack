import { FrameworkConfiguration, PLATFORM } from 'aurelia-framework';

export function configure(config: FrameworkConfiguration) {
  config.globalResources([
    PLATFORM.moduleName('./date-picker'),
    PLATFORM.moduleName('./loader'),
    PLATFORM.moduleName('./navbar')
  ]);
}
