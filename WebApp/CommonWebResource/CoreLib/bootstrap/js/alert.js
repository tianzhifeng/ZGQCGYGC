/**
 * Created by gw on 2015/7/24.
 */
angular.module('FeCloud.Alert', [])
    .directive('alert', function () {
        return {
            restrict:'EA',
            template:'<div class="md-modal md-effect"> '+
            '<div class="md-judge" ng-if="type==1"> <div class="judge-head"> <span>{{title}}</span> <div class="md-close fr" ng-click="close($event)"></div> </div> <p ng-transclude></p> <div class="delete" ng-click="close($event)">{{btntitle}}</div> </div>' +
            '<div class="md-tip tip-{{state}}" ng-if="type==0"><div ng-transclude></div> <div class="md-close" ng-click="close($event)"></div> </div>'+
            '<div class="md-box" ng-if="type==2"> <div class="box-head"> <span class="ch">{{title}}</span> <div class="md-close fl" ng-click="close($event)"></div> </div> <div class="box-line"></div> <div class="box-context" ng-transclude></div> <div class="box-btn"> <div> <button class="cbtn-cancel" type="submit" ng-click="close($event)">{{btnleft}}</button><button class="cbtn-ok" type="submit" ng-click="haddle($event)">{{btnright}}</button></div></div></div>'+
            '</div>',
            transclude:true,
            replace:true,
            scope: {
                type: '@',
                title:'@',
                btntitle:'@',
                btnleft:'@',
                btnright:'@',
                state:'@',
                close: '&',
                haddle:'&'
            }
        };
    }).service("alertService",function ($timeout){
        this.addAlert = function(alerts,state,msg) {
            state=state===0?"success":"error";
            alerts.push({type:0,msg: msg,state:state});
            $timeout(function(){
                alerts[alerts.length-1].check=true;
            },1);
        };

        this.addBox = function(alerts,title,btntitle,msg) {
            alerts.push({type:1,title:title,btntitle:btntitle,msg: msg});
            $timeout(function(){
                alerts[alerts.length-1].check=true;
            },1);
        };

        this.addchoosebox=function(alerts,title,btnleft,btnright,msg){
            alerts.push({type:2,title:title,btnleft:btnleft,btnright:btnright,msg: msg});
            $timeout(function(){
                alerts[alerts.length-1].check=true;
            },1);
        }

        this.closeAlert = function(alerts,index) {
            alerts[index].check=false;
            $timeout(function(){
                alerts.splice(index, 1);
            },500);
        };
    });