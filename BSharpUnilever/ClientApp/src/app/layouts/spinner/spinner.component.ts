import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'b-spinner',
  template: '<fa-icon icon="spinner" [pulse]="true"></fa-icon>',
})
export class SpinnerComponent {
}
