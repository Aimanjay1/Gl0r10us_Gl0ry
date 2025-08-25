
"use client";
import React, { useState } from "react";
import { useRouter } from "next/navigation";

export default function Profile() {
  const router = useRouter();
  const [profile, setProfile] = useState({
    fullName: "Muhammad Mustapha bin Karim",
    companyName: "Hong Ling Market",
    companyAddress: "No 23, Jalan Putra, 72300, Kelantan",
    companyEmail: "hongling@gmail.com",
    companyContact: "013-456 789",
    companyLogo: "hllogo.jpg",
    profilePic: "",
  });
  const [isEditMode, setIsEditMode] = useState(false);
  const [editProfile, setEditProfile] = useState(profile);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setEditProfile({ ...editProfile, [name]: value });
  };

  const handleProfilePicChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        setEditProfile({ ...editProfile, profilePic: reader.result });
      };
      reader.readAsDataURL(file);
    }
  };

  const handleLogoChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setEditProfile({ ...editProfile, companyLogo: file.name });
    }
  };

  const handleConfirm = () => {
    setProfile(editProfile);
    setIsEditMode(false);
  };

  const handleEdit = () => {
    setEditProfile(profile);
    setIsEditMode(true);
  };

  return (
    <>
      {/* Header & Avatar Section */}
      <div className="relative w-full" style={{ background: 'linear-gradient(to bottom, #fbb169 0%, #fbb169 60%, #fff 60%, #fff 100%)' }}>
        <div className="flex items-center px-4 pt-4">
          <button
            className="mr-2 text-white text-2xl font-bold cursor-pointer"
            aria-label="Back"
            onClick={() => router.push("/")}
          >
            &#8592;
          </button>
          <h1 className="text-2xl font-bold text-white flex-1 text-center mr-7">
            Profile
          </h1>
        </div>
        <div className="flex flex-col items-center justify-center" style={{ marginTop: '2rem' }}>
          <div className="w-40 h-40 rounded-full bg-orange-300 flex items-center justify-center text-5xl font-bold text-orange-700 border-4 border-white shadow-lg relative">
            {isEditMode ? (
              editProfile.profilePic ? (
                <img src={editProfile.profilePic} alt="Profile" className="w-full h-full rounded-full object-cover" />
              ) : (
                <span>A</span>
              )
            ) : profile.profilePic ? (
              <img src={profile.profilePic} alt="Profile" className="w-full h-full rounded-full object-cover" />
            ) : (
              <span>A</span>
            )}
            {isEditMode && (
              <label htmlFor="profilePicInput" className="absolute right-2 bottom-2 bg-white rounded-full p-2 cursor-pointer border border-gray-300">
                <span role="img" aria-label="edit">✎</span>
                <input id="profilePicInput" type="file" accept="image/*" style={{ display: 'none' }} onChange={handleProfilePicChange} />
              </label>
            )}
          </div>
          {isEditMode ? (
            <button className="bg-blue-500 text-white px-6 py-2 rounded mt-6 shadow" onClick={handleConfirm}>
              Confirm
            </button>
          ) : (
            <button className="bg-blue-500 text-white px-6 py-2 rounded mt-6 shadow" onClick={handleEdit}>
              Edit
            </button>
          )}
        </div>
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
              name="fullName"
              className="mt-1 w-full border rounded px-3 py-2"
              value={isEditMode ? editProfile.fullName : profile.fullName}
              onChange={isEditMode ? handleChange : undefined}
              readOnly={!isEditMode}
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700">
              Company Name
            </label>
            <input
              type="text"
              name="companyName"
              className="mt-1 w-full border rounded px-3 py-2"
              value={isEditMode ? editProfile.companyName : profile.companyName}
              onChange={isEditMode ? handleChange : undefined}
              readOnly={!isEditMode}
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700">
              Company Address
            </label>
            <textarea
              name="companyAddress"
              className="mt-1 w-full border rounded px-3 py-2"
              rows={2}
              value={isEditMode ? editProfile.companyAddress : profile.companyAddress}
              onChange={isEditMode ? handleChange : undefined}
              readOnly={!isEditMode}
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
                  name="companyEmail"
                  className="w-full bg-transparent"
                  value={isEditMode ? editProfile.companyEmail : profile.companyEmail}
                  onChange={isEditMode ? handleChange : undefined}
                  readOnly={!isEditMode}
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
                  name="companyContact"
                  className="w-full bg-transparent"
                  value={isEditMode ? editProfile.companyContact : profile.companyContact}
                  onChange={isEditMode ? handleChange : undefined}
                  readOnly={!isEditMode}
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
                name="companyLogo"
                className="w-full bg-transparent"
                value={isEditMode ? editProfile.companyLogo : profile.companyLogo}
                onChange={isEditMode ? handleChange : undefined}
                readOnly={!isEditMode}
              />
              {isEditMode && (
                <input type="file" accept="image/*" className="ml-2" onChange={handleLogoChange} />
              )}
            </div>
          </div>
        </form>
      </div>
    </>
  );
}
