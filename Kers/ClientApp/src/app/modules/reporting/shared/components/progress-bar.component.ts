import {  Component, Input } from '@angular/core';

@Component({
    selector: 'progress-bar',
    template: `
<div class="download-overlay" *ngIf="csvInitiated">
  <div class="text-right"><button class="btn btn-info btn-xs" (click)="csvInitiated=false">cancel</button></div>
<br>
<br>
  <h4>Generating Report</h4>
  <div class="progress">
    <div class="progress-bar progress-bar-striped bg-info" role="progressbar" [style.width.%]="batchesCompleted/(totalBatches - 1)*100"></div>
  </div>
  <div >
    <span *ngIf="averageBatchTime != 0">Time Remaining: {{getTimeRemaining() | number:'1.0-1' }} min.</span>&nbsp;
  </div>

</div>
    `,
    styles:[`
.download-overlay{
    background-color:rgba(220,239,230, 0.8);
    border: 3px solid rgba(120,139,130, 0.2);
    position: absolute;
    top:0;
    bottom:0;
    left:0;
    right:0;
    z-index: 100;
    padding: 10px;
  }



    `]
})

export class ProgressBarComponent{

    @Input('batchesCompleted') batchesCompleted = 0;
    @Input('totalBatches') totalBatches = 0;
    @Input('averageBatchTime') averageBatchTime = 0;
    

    getTimeRemaining():number{
        return (this.totalBatches - this.batchesCompleted ) * this.averageBatchTime / 60;
    }

}