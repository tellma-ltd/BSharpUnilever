import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../data/auth.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'b-confirm-email',
  templateUrl: './confirm-email.component.html',
  styles: []
})
export class ConfirmEmailComponent implements OnInit {

  private userId: string;
  private emailConfirmationToken: string;
  public showSpinner = false;
  public isConfirmed = false;
  public errorMessage: string;

  constructor(private auth: AuthService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.auth.signOut();

    const queryParamMap = this.route.snapshot.queryParamMap;
    this.emailConfirmationToken = queryParamMap.get('emailConfirmationToken');
    this.userId = queryParamMap.get('userId');

    if (!this.emailConfirmationToken || !this.userId) {
      this.errorMessage = `Sorry, the URL is missing one of the required parameters: 'emailConfirmationToken' or 'userId'`;
    } else {
      this.errorMessage = null;
      this.showSpinner = true;

      this.auth.confirmEmail(this.userId, this.emailConfirmationToken).subscribe(
        () => {
          this.showSpinner = false;
          this.isConfirmed = true;
        },
        (err) => {
          this.showSpinner = false;
          this.errorMessage = err;
        }
      );
    }
  }
}
