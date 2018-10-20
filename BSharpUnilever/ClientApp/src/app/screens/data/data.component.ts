import { Component, OnInit, OnDestroy } from '@angular/core';
import { DataService } from '../../data/data.service';
import { Subject } from 'rxjs';
import { downloadBlob } from '../../misc/util';

@Component({
  selector: 'b-data',
  templateUrl: './data.component.html',
  styles: []
})
export class DataComponent implements OnInit, OnDestroy {

  private notifyDestruct$ = new Subject<void>();

  public showSpinner = false;
  public errorMessage: string;

  constructor(private data: DataService) { }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.notifyDestruct$.next();
  }

  onDownload() {
    this.showSpinner = true;
    this.errorMessage = null;
    this.data.supportrequests.getData(this.notifyDestruct$).subscribe(
      (blob: Blob) => {
        this.showSpinner = false;
        const fileName = `Support Requests ${new Date().toDateString()}.xlsx`;
        downloadBlob(blob, fileName);
      },
      (friendlyError: any) => {
        this.showSpinner = false;
        this.errorMessage = friendlyError;
      }
    );
  }
}
