import '../models/auth_response.dart';
import '../models/user_credentials.dart';

abstract class AuthRepository {
  Future<AuthResponse> login(UserCredentials credentials);
  Future<void> logout();
  Future<bool> isAuthenticated();
}
