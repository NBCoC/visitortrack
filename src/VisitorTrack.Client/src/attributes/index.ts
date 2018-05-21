import { FrameworkConfiguration, PLATFORM } from 'aurelia-framework';

export function configure(config: FrameworkConfiguration) {
  config.globalResources([PLATFORM.moduleName('./loading-form-indicator'), PLATFORM.moduleName('./loading-indicator')]);
}
