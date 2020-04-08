#include "gamemaker.h"

namespace gamemaker {
	namespace internal {
		void(*gamemaker_perform_async_event)(int map_id, int event_type_id) = NULL;
		int(*gamemaker_ds_map_create)(int num, ...) = NULL;
		bool(*gamemaker_ds_map_add_double)(int map_id, char* key, double val) = NULL;
		bool(*gamemaker_ds_map_add_string)(int map_id, char* key, char* val) = NULL;
	}
	

	EXPORT
	void RegisterCallbacks(void* arg1, void* arg2, void* arg3, void* arg4)
	{
		internal::gamemaker_perform_async_event = (void(*)(int, int))(arg1);
		internal::gamemaker_ds_map_create = (int(*)(int, ...)) (arg2);
		internal::gamemaker_ds_map_add_double = (bool(*)(int, char*, double))(arg3);
		internal::gamemaker_ds_map_add_string = (bool(*)(int, char*, char*))(arg4);
	}

	ds_map::ds_map() {
		id = internal::gamemaker_ds_map_create(0);
	}

	ds_map::ds_map(double type_id) {
		id = internal::gamemaker_ds_map_create(0);

		set(type_key, type_id);
	}

	ds_map::ds_map(char* type_key, double type_id) {
		id = internal::gamemaker_ds_map_create(0);

		set(type_key, type_id);
	}

	bool ds_map::set(const char* key, double val) {
		if (id == -1) {
			return false;
		}

		return internal::gamemaker_ds_map_add_double(id, (char*)key, val);
	}

	bool ds_map::set(const char* key, const char* val) {
		if (id == -1) {
			return false;
		}

		return internal::gamemaker_ds_map_add_string(id, (char*)key, (char*)val);
	}
	
	bool ds_map::dispatch(gml_event_type event_type_id) {
		if (id == -1) {
			return false;
		}

		internal::gamemaker_perform_async_event(id, event_type_id);

		return true;
	}

	double ds_map::get_id() {
		return id;
	}
}