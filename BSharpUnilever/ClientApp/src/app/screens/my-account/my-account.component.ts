import { Component } from '@angular/core';
import { AuthService } from '../../data/auth.service';
import { GlobalsResolverService } from '../../data/globals-resolver.service';

@Component({
  selector: 'b-my-account',
  templateUrl: './my-account.component.html',
  styles: []
})
export class MyAccountComponent {

  constructor(private globals: GlobalsResolverService, private auth: AuthService) { }

  public currentPassword: string;
  public newPassword: string;
  public confirmNewPassword: string;
  public showSpinner = false;
  public errorMessage: string = null;
  public successMessage: string = null;

  get fullName(): string {
    return this.globals.currentUser.FullName;
  }

  get role(): string {
    return this.globals.currentUser.Role;
  }

  get email(): string {
    return this.globals.currentUser.Email;
  }

  onChangePassword() {
    if (!this.currentPassword) {
      this.errorMessage = 'Please specify the current password';

    } else if (!this.newPassword) {
      this.errorMessage = 'Please specify the new password';

    } else if (this.newPassword !== this.confirmNewPassword) {
      this.errorMessage = 'Make sure the passwords match';

    } else {
      this.showSpinner = true;
      this.errorMessage = null;
      this.successMessage = null;
      this.auth.changePassword(this.currentPassword, this.newPassword).subscribe(
        () => {
          this.showSpinner = false;
          this.currentPassword = null;
          this.newPassword = null;
          this.confirmNewPassword = null;
          this.successMessage = 'Your password was changed successfully';
        },
        (friendlyError: any) => {
          this.showSpinner = false;
          this.errorMessage = friendlyError;
        }
      );
    }
  }
}
