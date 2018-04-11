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

  public async activate(): Promise<void> {
    await this.registerValidation();
  }

  protected async validate(): Promise<boolean> {
    const result = await this.validationController.validate();
    return result.valid;
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
        target.classList.remove('is-danger');
        const parent = target.parentElement.parentElement;
        let element = parent.querySelector('.help.is-danger');
        if (!element) return;
        element.textContent = '';
      });
    }

    for (let { result, elements } of instruction.render) {
      elements.forEach(target => {
        if (result.valid) return;
        target.classList.add('is-danger');
        const parent = target.parentElement.parentElement;
        let element = parent.querySelector('.help.is-danger');
        if (!element) return;
        element.textContent = result.message;
      });
    }
  }
}
