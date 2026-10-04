import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { Model } from 'survey-core';

@Component({
    selector: 'survey-renderer',
    template: `<div class="survey-container contentcontainer codecontainer"><survey [model]="surveyModel"></survey></div>`,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class SurveyComponent  {
    @Output() submitSurvey = new EventEmitter<any>();
    @Input() json: any;
    @Input() previousResult: any;
    result: any;
    surveyModel: Model;

    ngOnInit() {
        const surveyModel = new Model(this.json);
        this.surveyModel = surveyModel;
        surveyModel.onAfterRenderQuestion.add((survey, options) => {
            const popupDescription = (options.question as any).popupdescription;
            if (!popupDescription) { return; }
            // Add a button;
            const btn = document.createElement('button');
            btn.className = 'btn btn-info btn-xs';
            btn.type = 'button';
            btn.textContent = 'More Info';
            btn.onclick = function () {
              alert(popupDescription);
            };
            const header = options.htmlElement.querySelector('h5');
            if (!header) { return; }
            const span = document.createElement('span');
            span.innerHTML = '  ';
            header.appendChild(span);
            header.appendChild(btn);
          });
        surveyModel.onComplete.add(sender => {
            this.submitSurvey.emit(sender.data);
            this.result = sender.data;
        });
        if(this.previousResult != null){
          surveyModel.mode = 'display';
          surveyModel.data = this.previousResult;
        }
    }
}