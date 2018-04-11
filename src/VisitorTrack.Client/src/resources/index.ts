import { FrameworkConfiguration, PLATFORM } from 'aurelia-framework';

export function configure(config: FrameworkConfiguration) {
  config.globalResources([
    PLATFORM.moduleName('./attributes/loading-indicator'),
    PLATFORM.moduleName('./attributes/loading-form-indicator'),
    PLATFORM.moduleName('./value-converters/filter-by'),
    PLATFORM.moduleName('./value-converters/sort-by'),
    PLATFORM.moduleName('./value-converters/group-by')
  ]);
}
