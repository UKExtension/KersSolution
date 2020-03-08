import { Component, OnInit, Input } from '@angular/core';
import { saveAs } from 'file-saver';
import { Training, TrainingEnrollment } from './training';
import { TrainingService } from './training.service';

@Component({
  selector: '[training-post-attendance-detail]',
  templateUrl: './training-post-attendance-detail.component.html',
  styles: []
})
export class TrainingPostAttendanceDetailComponent implements OnInit {

  @Input('training-post-attendance-detail') training:Training;

  default = true;
  post = false;
  survey = false;
  loading = false;
  enrolledFolks: TrainingEnrollment[];
  moreInfo = false;
  moreInfoSurvey = false;
  coppied = false;

  constructor(
    private service:TrainingService
  ) { }

  ngOnInit() {
    this.enrolledFolks = this.training.enrollment.filter( e => e.eStatus == "E").sort((a, b) => a.attendie.rprtngProfile.name.localeCompare(b.attendie.rprtngProfile.name));
  }

  defaultView(){
    this.default = true;
    this.post = false;
    this.survey = false;
  }
  postView(){
    this.default = false;
    this.post = true;
  }
  evaluationView(){
    this.survey = true;
    this.default = false;
  }
  checked(event:any, enrolled:TrainingEnrollment){
    if(event.currentTarget.checked){
      enrolled.attended = true;
    }else{
      enrolled.attended = false;
    }
  }
  public notify(payload: string) {
    // Might want to notify the user that something has been pushed to the clipboard
    //console.info(`'${payload}' has been copied to clipboard`);
    this.coppied = true;
  }
  submit(){
    this.loading = true;
    this.service.updateAttendance(this.training.id, this.training).subscribe(
      res => {
        this.loading = false;
        this.training = res;
        this.defaultView();
      }
    );
  }
  csv(){
    var startDate = new Date(this.training.start);
    var filename = (startDate.getMonth() + 1) + "_" + startDate.getDate() + "_" + startDate.getFullYear();
    filename += "_" + this.training.subject.replace(" ", "_").substr(0,10) + ".csv";
    var data = this.csvHeader();
    for( var result of this.training.surveyResults){
      data.push( this.csvRow(JSON.parse(result.result )));
    }
    const replacer = (key, value) => value === null ? '' : value; // specify how you want to handle null values here
    const header = Object.keys(data[0]);
    let csv = data.map(row => header.map(fieldName => JSON.stringify(row[fieldName], replacer)).join(','));
    //csv.unshift(header.join(','));
    let csvArray = csv.join('\r\n');
    var blob = new Blob([csvArray], {type: 'text/csv' })
    saveAs(blob, filename);
  }

  csvHeader(){
    var returnVal = ["1. Content - 1. Was relevant to my needs."];
    returnVal.push("1. Content - 2. Was well organized.");
    returnVal.push("1. Content - 3. Was adequately related to the topic.");
    returnVal.push("1. Content - 4. Was easy to understand.");
    returnVal.push("2. Instructors - 1. Were well-prepared.");
    returnVal.push("2. Instructors - 2. Used teaching methods appropriate for the content/audience.");
    returnVal.push("2. Instructors - 3. Was knowledgeable of the subject matter.");
    returnVal.push("2. Instructors - 4. Engaged the participants in learning.");
    returnVal.push("2. Instructors - 5. Related program content to practical situations.");
    returnVal.push("2. Instructors - 6. Answered questions clearly and accurately.");
    returnVal.push("3. Outcomes - 1. I gained knowledge/skills about the topics presented.");
    returnVal.push("3. Outcomes - 2. I will use what I learned in my county program.");
    returnVal.push("3. Outcomes - 3. This information will help my program move to the next level.");
    returnVal.push("3. Outcomes - 4. Based on the in-service, I am now able to teach this topic to others.");
    returnVal.push("4. Based on the in-service, I am now able to teach this topic to others.");
    returnVal.push("5. Based on this in-service, what are two things that you are encouraged to do within the next six (6) months?");
    returnVal.push("6. If you have a program related to this topic, what do you think will help take it to the next level (i.e., achieve higher level impact)?");
    returnVal.push("7. Please provide any additional comments about this training.");
    returnVal.push("8. Please provide any comments about the instructor or any additional instructors/presenters.");
    return [returnVal];
  }

  csvRow(result:object):string[]{
    var ret = [];
    for( var i = 1; i<5; i++) result["1. Content"] != undefined ? ret.push(result["1. Content"]["1."+i]):"";
    for( var i = 1; i<7; i++) result["2. Instructors"] != undefined ? ret.push(result["2. Instructors"]["2."+i]) : "";
    for( var i = 1; i<5; i++) result["3. Outcomes"] != undefined ? ret.push(result["3. Outcomes"]["3."+i]) : "";
    ret.push(result["4. Teach"]);
    ret.push(result["5. Encouraged"]);
    ret.push(result["6. Program"]);
    ret.push(result["7. Training Comments"]);
    ret.push(result["8. Instructors"]);
    
    return ret;
  }


}
