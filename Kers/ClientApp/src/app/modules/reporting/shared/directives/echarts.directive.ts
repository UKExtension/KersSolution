import {
  Directive, ElementRef, Input, OnInit, HostBinding, OnChanges, OnDestroy, Output, EventEmitter
} from '@angular/core';

import {Subject, Subscription} from "rxjs";

import * as echarts from 'echarts';
import ECharts = echarts.ECharts;
import EChartOption = echarts.EChartOption;


@Directive({
  selector: '[ts-chart]',
})
export class echartsDirective implements OnChanges,OnInit,OnDestroy {
  private chart: ECharts;
  private sizeCheckInterval = null;
  private reSize$ = new Subject<string>();
  private onResize: Subscription;

  @Input('ts-chart') options: EChartOption;




  // chart events:
  @Output() chartInit: EventEmitter<any> = new EventEmitter<any>();
  @Output() chartClick: EventEmitter<any> = new EventEmitter<any>();
  @Output() chartDblClick: EventEmitter<any> = new EventEmitter<any>();
  @Output() chartMouseDown: EventEmitter<any> = new EventEmitter<any>();
  @Output() chartMouseUp: EventEmitter<any> = new EventEmitter<any>();
  @Output() chartMouseOver: EventEmitter<any> = new EventEmitter<any>();
  @Output() chartMouseOut: EventEmitter<any> = new EventEmitter<any>();
  @Output() chartGlobalOut: EventEmitter<any> = new EventEmitter<any>();
  @Output() chartContextMenu: EventEmitter<any> = new EventEmitter<any>();
  @Output() chartDataZoom: EventEmitter<any> = new EventEmitter<any>();


  @HostBinding('style.height.px')
  elHeight: number;

  constructor(private el: ElementRef) {
    this.chart = echarts.init(this.el.nativeElement, 'vintage');
  }

  private registerEvents(myChart: any) {
    if (myChart) {
      // register mouse events:
      myChart.on('click', (e: any) => { this.chartClick.emit(e); });
      myChart.on('dblClick', (e: any) => { this.chartDblClick.emit(e); });
      myChart.on('mousedown', (e: any) => { this.chartMouseDown.emit(e); });
      myChart.on('mouseup', (e: any) => { this.chartMouseUp.emit(e); });
      myChart.on('mouseover', (e: any) => { this.chartMouseOver.emit(e); });
      myChart.on('mouseout', (e: any) => { this.chartMouseOut.emit(e); });
      myChart.on('globalout', (e: any) => { this.chartGlobalOut.emit(e); });
      myChart.on('contextmenu', (e: any) => { this.chartContextMenu.emit(e); });
      
      // other events;
      myChart.on('dataZoom', (e: any) => { this.chartDataZoom.emit(e); });
    }
  }


  ngOnChanges(changes) {
    if (this.options) {
      this.chart.setOption(this.options);
      this.registerEvents(this.chart);
    }
  }

  ngOnInit() {
    this.sizeCheckInterval = setInterval(() => {
      this.reSize$.next(`${this.el.nativeElement.offsetWidth}:${this.el.nativeElement.offsetHeight}`)
    }, 100);
    //this.onResize = this.reSize$.distinctUntilChanged().subscribe((_) => this.chart.resize());

    this.elHeight = this.el.nativeElement.offsetHeight;
    if (this.elHeight < 300) {
      this.elHeight = 300;
    }
  }


  ngOnDestroy() {
    if (this.sizeCheckInterval) {
      clearInterval(this.sizeCheckInterval);
    }
    this.reSize$.complete();
    if (this.onResize) {
      this.onResize.unsubscribe();
    }
  }
}