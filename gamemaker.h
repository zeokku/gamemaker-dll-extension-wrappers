#pragma once

#define NULL 0
#define EXPORT extern "C" __declspec(dllexport)
#ifdef ENABLE_LOG
#define LOG(...) { printf(__VA_ARGS__); printf("\n"); fflush(stdout); }
#else
#define LOG(...) {}
#endif

namespace gamemaker {
	enum gml_event_type : int {
		steam = 69,
		social = 70,
		system = 75
	};

	class ds_map {

	private:
		char* type_key = (char*)"_type_";

	public:
		int id = -1;

		ds_map();
		ds_map(double);
		ds_map(char*, double);
		
		bool set(const char*, double);
		bool set(const char*, const char*);

		bool dispatch(gml_event_type);

		double get_id();
	};
}