import { Component, OnInit, OnDestroy } from '@angular/core';
import { DataService } from '../../data/data.service';
import { Subject } from 'rxjs';

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

        // Create an in memory url for the blob, further reading:
        // https://developer.mozilla.org/en-US/docs/Web/API/URL/createObjectURL
        var url = window.URL.createObjectURL(blob);

        // Below is a trick for downloading files without opening
        // a new window. This is a more elegant user experience
        var a = document.createElement('a');
        document.body.appendChild(a);
        a.setAttribute('style', 'display: none');
        a.href = url;
        a.download = `Support Requests ${new Date().toDateString()}.xlsx`;
        a.click();
        a.remove();

        // Best practice to prevent a memory leak, especially in a SPA like bSharp
        window.URL.revokeObjectURL(url);
      },
      (friendlyError: any) => {
        this.showSpinner = false;
        this.errorMessage = friendlyError;
      }
    );
  }
}
