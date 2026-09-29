import { ModuleWithProviders, NgModule } from '@angular/core';
import { FroalaEditorDirective } from './froala-editor.directive';
import { FroalaViewDirective } from './froala-view.directive';

@NgModule({
  declarations: [FroalaEditorDirective],
  exports: [FroalaEditorDirective]
})
export class FroalaEditorModule {
  static forRoot(): ModuleWithProviders<FroalaEditorModule> {
    return { ngModule: FroalaEditorModule, providers: [] };
  }
}

@NgModule({
  declarations: [FroalaViewDirective],
  exports: [FroalaViewDirective]
})
export class FroalaViewModule {
  static forRoot(): ModuleWithProviders<FroalaViewModule> {
    return { ngModule: FroalaViewModule, providers: [] };
  }
}
