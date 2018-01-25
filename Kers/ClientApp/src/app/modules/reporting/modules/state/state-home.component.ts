import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';
import {StateService} from './state.service';


import * as echarts from 'echarts';
import { CountyService } from '../county/county.service';

@Component({
  template: `
  <!--
  <div [ts-chart]="option" (chartClick)="ccc($event)"></div>
  -->
    <router-outlet></router-outlet>
    
  `
})
export class StateHomeComponent { 

    errorMessage:string;

    option;

    geoCoordMap = {};


    populationData = [];

    constructor( 
        private reportingService: ReportingService,
        private service:StateService,
        private countyService: CountyService
    )   
    {}

    ccc(event){
        console.log(event);
    }


    convertData(data) {
        var res = [];
        for (var i = 0; i < data.length; i++) {
            var geoCoord = this.geoCoordMap[data[i].name];
            if (geoCoord) {
                res.push({
                    name: data[i].name,
                    value: geoCoord.concat(data[i].value)
                });
            }
        }
        return res;
    }



    ngOnInit(){
        
        this.defaultTitle();


        this.service.kyMap().subscribe(
            res => {


                echarts.registerMap('KY', res);
            
                this.service.counties().subscribe(
                    res => {
                        var data = []
                        for( var cnt of res){
                            if(cnt.name.substring(cnt.name.length - 3) == "CES"){
                                data.push(
                                    {
                                        name: cnt.name,
                                        value: cnt.population
                                    }
                                );
                                if(cnt.geoFeature != undefined){
                                    var go = JSON.parse(cnt.geoFeature);
                                    var center = this.countyService.geoCenter(go.geometry.coordinates[0][0]);
                                    this.geoCoordMap[cnt.name] = center;
                                }else{
                                    this.geoCoordMap[cnt.name] = [0,0];
                                }
                            }
                            
                        }
                        
                        var convertedData = this.convertData(data);
                        
                        this.option = {
                            title: {
                                text: 'Kentucky',
                                subtext: '（Population by County）',
                        
                                x: 'center',
                                textStyle: {
                                    color: '#424242'
                                }
                            },
                            tooltip: {
                                show: false
                            },
                            visualMap: {
                                min: 0,
                                max: 1500,
                                left: '-100%',
                                top: 'bottom',
                                text: ['High', 'Low'],
                                seriesIndex: [1],
                                inRange: {
                                    color: ['#e0ffff', '#006edd']
                                },
                                calculable: false
                            },
                            geo: {
                                map: 'KY',
                                roam: 'move',
                                scaleLimit:{
                                    max:'1.9',
                                    min:'0.9'
                                },
                                label: {
                                    normal: {
                                        show: true,
                                        textStyle: {
                                            color: 'rgba(0,0,0,0.6)'
                                        }
                                    }
                                },
                                itemStyle: {
                                    normal: {
                                        borderColor: 'rgba(0, 0, 0, 0.2)'
                                    },
                                    emphasis: {
                                        areaColor: null,
                                        shadowOffsetX: 0,
                                        shadowOffsetY: 0,
                                        shadowBlur: 20,
                                        borderWidth: 0,
                                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                                    }
                                }
                            },
                            series: [{
                                type: 'scatter',
                                coordinateSystem: 'geo',
                                data: convertedData,
                                symbolSize: 20,
                                symbol: 'path://M35.025,17.608c-5.209-4.786-11.483-2.301-15.303-4.281v-1.482c0-0.41-0.333-0.743-0.743-0.743c-0.411,0-0.743,0.333-0.743,0.743v24.682c-1.855,0.104-3.261,0.59-3.261,1.175c0,0.661,1.793,1.197,4.005,1.197c2.21,0,4.003-0.536,4.003-1.197c0-0.585-1.405-1.071-3.261-1.175V26.151C24.575,24.573,28.408,17.166,35.025,17.608z',
                                symbolRotate: 0,
                                symbolOffset: ['50%', '-100%'],
                                label: {
                                    normal: {
                                        formatter: '{b}',
                                        position: 'top',
                                        show: false,
                                        textStyle: {
                                            color: '#000000',
                                            fontSize: 16
                                        }
                        
                                    },
                                    emphasis: {
                                        show: false
                                    }
                                },
                                itemStyle: {
                                    normal: {
                                        color: '#F06C00'
                                    }
                                }
                            }, {
                                name: '个人会员数量',
                                type: 'map',
                                geoIndex: 0,
                                tooltip: {
                                    show: true
                                },
                                data: data
                            }]
                        };















                    }
                );


                            },
                            err => this.errorMessage = <any> err
                        );



























        /*
        this.service.kyMap().subscribe(
            res => {
                this.service.counties().subscribe(
                    counts => {
                        for( let ftr of res.features){
                            var cnt = counts.filter(c => c.name.substring(0, c.name.length - 11) == ftr.properties.name);
                            if(cnt.length > 0){
                                this.service.addGeoFeature(cnt[0].id, ftr).subscribe();
                            }
                        }
                    }
                )
                
            },
            err=> this.errorMessage = <any>err
        );
        */

        /*

        {"type": "FeatureCollection", "features": }
        this.service.kyMap().subscribe(
            res => {


                echarts.registerMap('KY', res);


                this.service.kyPopulationByCounty().subscribe(
                    res => {
                        var i = 0;
                        for(var row of res){
                            if(i != 0){
                                this.populationData.push({name:row[1].substring(0, row[1].length - 17 ), value: +row[0] })
                            }
                            i++;
                        }
                        

                        this.option = {
                            title: {
                                text: 'Kentucky Population Estimates (2016)',
                                subtext: 'Data from www.census.gov',
                                sublink: 'http://www.census.gov/popest/data/datasets.html',
                                left: 'right'
                            },


                            
                            "geo": {
                            
                                "map": "KY",
                                
                            },


                           
                            toolbox: {
                                show: true,
                                //orient: 'vertical',
                                left: 'left',
                                top: 'top',
                                feature: {
                                    dataView: {readOnly: false},
                                    restore: {},
                                    saveAsImage: {}
                                }
                            },
                            series: [
                                {
                                    name: 'Kentucky PopEstimates',
                                    type: "effectScatter",
                                    coordinateSystem: "geo",


                                     tooltip: {
                                        trigger: 'item',
                                        showDelay: 0,
                                        transitionDuration: 0.2,
                                        formatter: function (params) {
                                            var value = (params.value + '').split('.');
                                            var val = value[0].replace(/(\d{1,3})(?=(?:\d{3})+(?!\d))/g, '$1,');
                                            return params.seriesName + '<br/>' + params.name + ': ' + val;
                                        }
                                    },

                                    data:this.populationData                                
                                }
                            ]
                    };






                    },
                    err => this.errorMessage = <any> err
                )
                





            },
            err => this.errorMessage = <any> err
        )
        */
    }

    defaultTitle(){
        this.reportingService.setTitle("State Admin Dashboard");
    }
}