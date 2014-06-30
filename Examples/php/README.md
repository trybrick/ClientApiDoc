Examples in PHP
==============

PHP, being an extremely flexible and feature-rich language, provides a number of different mechanisms you can choose from to make your REST requests. The ones we recommend are:

- [cURL](http://php.net/manual/en/book.curl.php)
- [file_get_contents](http://php.net/manual/en/function.file-get-contents.php)

Depending on your web server configuration either one of these functions may be more desirable.

JSON (JavaScript Object Notation) is a lightweight data-interchange format. It is like XML, but without the markup around the actual payload.

The [json_decode](http://php.net/manual/en/function.json-decode.php) function takes a JSON encoded string and converts it into a PHP variable.

Examples

```php
<?php
  include 'gsnapiclient.php'; 
  $base_api_url = "https://clientapi.gsn2.com/api/v1";
  $client_id = "123";
  $client_secret = "";
  $apiClient = new GsnApiClient($base_api_url, $client_id, $client_secret);
  $apiClient->authenticate();
	
  // list stores
  $stores = $apiClient->get("/store/list/".$client_id);
	
  // retrieve profile
  $profile = $apiClient->get("/profile/by", false, array("site_id: ".$client_id));
	
  // update profile
  $profile->FirstName = "Tom";
  $updatedProfile = $apiClient->post("/profile/update", get_object_vars($profile), array("site_id: ".$client_id));
?>
```


