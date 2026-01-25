import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../storage/storage_service.dart';

part 'dio_client.g.dart';

@riverpod
Dio dio(DioRef ref) {
  final storageService = ref.watch(storageServiceProvider);
  
  final dio = Dio(
    BaseOptions(
      baseUrl: 'http://localhost:5000', // TODO: Move to config/env
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 10),
      contentType: 'application/json',
    ),
  );

  dio.interceptors.add(
    InterceptorsWrapper(
      onRequest: (options, handler) async {
        final token = await storageService.getToken();
        if (token != null) {
          options.headers['Authorization'] = 'Bearer $token';
        }
        return handler.next(options);
      },
      onError: (DioException e, handler) {
        if (e.response?.statusCode == 401) {
          // TODO: Handle logout or token refresh
        }
        return handler.next(e);
      },
    ),
  );

  return dio;
}
