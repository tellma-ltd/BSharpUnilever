import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../data/auth.service';

@Component({
  selector: 'b-reset-password-control',
  templateUrl: './reset-password-control.component.html',
  styles: []
})
export class ResetPasswordControlComponent implements OnInit {

  private passwordResetToken: string;
  private userId: string;
  public showSpinner: boolean;
  public password: string;
  public confirmPassword: string;
  public errorMessage: string;

  constructor(private auth: AuthService, private route: ActivatedRoute) { }

  ngOnInit() {
    // This screen can potentially sign a new user in, so sign out
    // any existing user to ensure consistent behavior
    this.auth.signOut();

    // Retrieve the user id and the token from the URL
    const queryParamsMap = this.route.snapshot.queryParamMap;
    this.passwordResetToken = queryParamsMap.get('passwordResetToken');
    this.userId = queryParamsMap.get('userId');

    // Just in case...
    if (!this.passwordResetToken || !this.userId) {
      this.errorMessage = `The password reset link is malformed`;
    }
  }

  onResetPassword() {
    this.errorMessage = null;

    if (this.password !== this.confirmPassword) {
      this.errorMessage = `Make sure the passwords match`;
    } else {
      this.auth.resetPassword(this.userId, this.passwordResetToken, this.password).subscribe(
        () => {
          this.showSpinner = false;
        },
        (err) => {
          this.showSpinner = false;
          this.errorMessage = err;
        }
      );
    }
  }
}
