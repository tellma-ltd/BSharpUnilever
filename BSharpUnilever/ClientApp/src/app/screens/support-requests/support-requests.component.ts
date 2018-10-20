import { Component, OnInit } from '@angular/core';
import { SupportRequest, supportRequestReasons } from '../../data/entities/SupportRequest';
import { GlobalsResolverService } from '../../data/globals-resolver.service';

@Component({
  selector: 'b-support-requests',
  templateUrl: './support-requests.component.html',
  styles: []
})
export class SupportRequestsComponent implements OnInit {

  constructor(private globals: GlobalsResolverService) { }

  ngOnInit() {
  }

  public requestedValue(supportRequest: SupportRequest) {
    let result: number = null;
    if (!!supportRequest.LineItems) {
      result = 0;
      for (let i = 0; i < supportRequest.LineItems.length; i++) {
        result += supportRequest.LineItems[i].RequestedValue;
      }
    }
    return result;
  }

  getReasonDisplay(key: string): string {
    return supportRequestReasons[key];
  }

  get showMyBalance(): boolean {
    return this.globals.currentUser.Role === 'KAE';
  }
}
