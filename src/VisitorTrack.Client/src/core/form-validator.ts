import {
  ValidationControllerFactory,
  ValidationController,
  RenderInstruction,
  validateTrigger,
  ValidationRenderer
} from 'aurelia-validation';

export abstract class FormValidator {
  private validationController: ValidationController;

  constructor(factory: ValidationControllerFactory) {
    this.validationController = factory.createForCurrentScope();
    this.validationController.addRenderer(new CustomValidationRenderer());
    this.validationController.validateTrigger = validateTrigger.change;
  }

  protected async validate(): Promise<boolean> {
    return (await this.validationController.validate()).valid;
  }

  protected async registerValidation(): Promise<void> {
    this.registerValidationRules();
    await this.validationController.validate();
  }

  protected abstract registerValidationRules(): void;
}

class CustomValidationRenderer implements ValidationRenderer {
  public render(instruction: RenderInstruction): void {
    for (let { elements } of instruction.unrender) {
      elements.forEach(target => {
        let element = target.parentElement.querySelector('.u-error');
        if (!element) return;
        element.textContent = '';
      });
    }

    for (let { result, elements } of instruction.render) {
      elements.forEach(target => {
        if (result.valid) return;
        let element = target.parentElement.querySelector('.u-error');
        if (!element) return;
        element.textContent = result.message;
      });
    }
  }
}
