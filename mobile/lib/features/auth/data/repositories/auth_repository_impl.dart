import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/dio_client.dart';
import '../../../../core/storage/storage_service.dart';
import '../../domain/models/auth_response.dart';
import '../../domain/models/user_credentials.dart';
import '../../domain/repositories/auth_repository.dart';

part 'auth_repository_impl.g.dart';

class AuthRepositoryImpl implements AuthRepository {
  final Dio _dio;
  final StorageService _storageService;

  AuthRepositoryImpl(this._dio, this._storageService);

  @override
  Future<AuthResponse> login(UserCredentials credentials) async {
    try {
      final response = await _dio.post(
        '/api/auth/login',
        data: credentials.toJson(),
      );
      
      final authResponse = AuthResponse.fromJson(response.data);
      await _storageService.saveToken(authResponse.token);
      return authResponse;
    } on DioException catch (e) {
      throw Exception(e.response?.data['message'] ?? 'Login failed');
    }
  }

  @override
  Future<void> logout() async {
    await _storageService.deleteToken();
  }

  @override
  Future<bool> isAuthenticated() async {
    final token = await _storageService.getToken();
    return token != null;
  }
}

@riverpod
AuthRepository authRepository(AuthRepositoryRef ref) {
  return AuthRepositoryImpl(
    ref.watch(dioProvider),
    ref.watch(storageServiceProvider),
  );
}
