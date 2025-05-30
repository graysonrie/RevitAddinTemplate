"use client";
import { useState } from "react";
import { invoke } from "./interop";

export default function Home() {
  const [views, setViews] = useState<string[]>([]);
  const buttonClick = async () => {
    console.log("Button clicked");
    const result = await invoke<string[]>(
      "GetViews", 
      { Args: "Hello" }, 
      ["View1", "View2", "View3"],
    );

    if (result.type === "ok") {
      if (Array.isArray(result.value)) {
        setViews(result.value);
        console.log("Success:", result.value);
      } else {
        console.error("Expected array but got:", typeof result.value);
        setViews([]);
      }
    } else {
      console.error("Error:", result.error);
      setViews([]);
    }
  };

  return (
    <div className="flex flex-row items-center justify-center h-screen gap-2">
      <div className="flex flex-col items-center justify-center">
        <button
          className="bg-blue-500 text-white p-2 rounded-md"
          onClick={buttonClick}
        >
          Get Views
        </button>
      </div>
      <div className="flex flex-col items-center justify-center">
        {Array.isArray(views) ? (
          views.map((item) => (
            <div key={item} className="text-2xl">{item}</div>
          ))
        ) : (
          <div className="text-red-500">Invalid views data</div>
        )}
      </div>
    </div>
  );
}
