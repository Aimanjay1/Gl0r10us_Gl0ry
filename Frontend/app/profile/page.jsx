"use client";

export default function Profile() {
  return (
    <>
      {/* Header & Avatar Section */}
      <div className="relative w-full" style={{ background: 'linear-gradient(to bottom, #fbb169 0%, #fbb169 60%, #fff 60%, #fff 100%)' }}>
        <div className="flex items-center px-4 pt-4">
          <button
            className="mr-2 text-white text-2xl font-bold cursor-pointer"
            aria-label="Back"
          >
            &#8592;
          </button>
          <h1 className="text-2xl font-bold text-white flex-1 text-center mr-7">
            Profile
          </h1>
        </div>
        <div className="flex flex-col items-center justify-center" style={{ marginTop: '2rem' }}>
          <div className="w-40 h-40 rounded-full bg-orange-300 flex items-center justify-center text-5xl font-bold text-orange-700 border-4 border-white shadow-lg">
            A
          </div>
          <button className="bg-blue-500 text-white px-6 py-2 rounded mt-6 shadow">
            Edit
          </button>
        </div>
        {/* Spacer to push content below avatar */}
        <div style={{ height: '3rem' }}></div>
      </div>

      {/* Profile Card */}
      <div className="max-w-md mx-auto mt-4 bg-white rounded-xl shadow-lg p-6">
        <form className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700">
              Full name
            </label>
            <input
              type="text"
              className="mt-1 w-full border rounded px-3 py-2"
              value="Muhammad Mustapha bin Karim"
              readOnly
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700">
              Company Name
            </label>
            <input
              type="text"
              className="mt-1 w-full border rounded px-3 py-2"
              value="Hong Ling Market"
              readOnly
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700">
              Company Address
            </label>
            <textarea
              className="mt-1 w-full border rounded px-3 py-2"
              rows={2}
              value=""
              readOnly
            />
          </div>
          <div className="flex gap-4">
            <div className="flex-1">
              <label className="block text-sm font-medium text-gray-700">
                Company Email
              </label>
              <div className="flex items-center border rounded px-3 py-2 mt-1">
                <span className="mr-2">📧</span>
                <input
                  type="email"
                  className="w-full bg-transparent"
                  value="hongling@gmail.com"
                  readOnly
                />
              </div>
            </div>
            <div className="flex-1">
              <label className="block text-sm font-medium text-gray-700">
                Company Contact Number
              </label>
              <div className="flex items-center border rounded px-3 py-2 mt-1">
                <span className="mr-2">📞</span>
                <input
                  type="text"
                  className="w-full bg-transparent"
                  value="013-456 789"
                  readOnly
                />
              </div>
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700">
              Company Logo
            </label>
            <div className="flex items-center border rounded px-3 py-2 mt-1">
              <span className="mr-2">📤</span>
              <input
                type="text"
                className="w-full bg-transparent"
                value="hllogo.jpg"
                readOnly
              />
            </div>
          </div>
        </form>
      </div>
    </>
  );
}
