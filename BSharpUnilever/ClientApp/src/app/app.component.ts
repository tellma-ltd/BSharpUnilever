import { Component } from '@angular/core';
import { DataService } from './data/data.service';

@Component({
  selector: 'b-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  constructor(private data: DataService) { }

  get isSaving(): boolean {
    return this.data.isSaving;
  }
}
