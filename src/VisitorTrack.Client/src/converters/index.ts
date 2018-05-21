import { FrameworkConfiguration, PLATFORM } from 'aurelia-framework';

export function configure(config: FrameworkConfiguration) {
  config.globalResources([
    PLATFORM.moduleName('./filter'),
    PLATFORM.moduleName('./sort'),
    PLATFORM.moduleName('./group'),
    PLATFORM.moduleName('./date-format')
  ]);
}
