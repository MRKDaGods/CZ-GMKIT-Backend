#define _CRT_SECURE_NO_WARNINGS

#include "Tokens.h"
#include "Concat.hpp"

#include <filesystem>
#include <fstream>
#include <Windows.h>

#define LOG(x) __log += concat(x); MRKLog(concat(x))
#define LOGN(x) LOG(concat(x, '\n'))

#define MRK_PATCH_CTX_SIG 0
#define MRK_PATCH_CTX_BODY 1

#define mrks _STD

namespace MRK {
	mrks string ms_LogPath;

	void MRKSetLogPath(mrks string path) {
		ms_LogPath = path;
	}

	void MRKLog(mrks string log, bool clear = false, bool sep = true) {
		_STD ofstream stream(ms_LogPath.c_str(), clear ? _STD ios_base::out : _STD ios_base::app);
		if (sep)
			stream << '\n';

		stream << log;
		stream.close();
	}

	enum MRKPatchType {
		MRK_PATCH_NONE = 0,
		MRK_PATCH_METHOD_SIG = 1,
		MRK_PATCH_METHOD_BODY = 2,
		MRK_PATCH_FIELD = 4
	};

	struct MRKPatch {
		int Type;
		char* Owner; //filename.cs
		char** Ctx; //void xyz(int i)[]
		char** New; //int xyz(void*)[]
	};

	template<typename T>
	struct SubVec : public _STD vector<T> {
	public:
		SubVec(_STD vector<T>& source, size_t start, size_t len) {
			for (size_t i = 0; i < len; i++)
				this->push_back(source[start + i]);
		}
	};

	bool TokenCmpDirect(Token& t1, Token& t2) {
		//direct value cmp
		switch (t1.ContextualKind) {

		case TOKEN_CONTEXTUAL_KIND_CHAR:
			return t1.Value.CharValue != t2.Value.CharValue;

		case TOKEN_CONTEXTUAL_KIND_IDENTIFIER:
			return t1.Value.IdentifierValue != t2.Value.IdentifierValue;

		case TOKEN_CONTEXTUAL_KIND_INT:
			return t1.Value.IntValue != t2.Value.IntValue;

		case TOKEN_CONTEXTUAL_KIND_LONG:
			return t1.Value.LongValue != t2.Value.LongValue;

		case TOKEN_CONTEXTUAL_KIND_SHORT:
			return t1.Value.ShortValue != t2.Value.ShortValue;

		case TOKEN_CONTEXTUAL_KIND_STRING:
			return t1.Value.StringValue != t2.Value.StringValue;

		case TOKEN_CONTEXTUAL_KIND_UINT:
			return t1.Value.UIntValue != t2.Value.UIntValue;
			
		case TOKEN_CONTEXTUAL_KIND_ULONG:
			return t1.Value.ULongValue != t2.Value.ULongValue;

		case TOKEN_CONTEXTUAL_KIND_USHORT:
			return t1.Value.UShortValue != t2.Value.UShortValue;

		}

		return true;
	}

	void ReplaceSig(_STD vector<Token>* owner, size_t start, size_t len, _STD vector<Token>& newSig) {
		for (size_t i = 0; i < len; i++) {
			owner->emplace(owner->begin() + start + i, newSig[i]);
		}
	}

	_STD string GetTokenOutBuffer(_STD vector<Token>& tokens) {
		_STD string str;
		for (Token& tk : tokens)
			str += Tokens::ToValueString(tk);

		return str;
	}

	void WriteTokensRaw(_STD string& path, _STD string raw) {
		_STD ofstream of(path, _STD ios_base::out);
		of << raw;
		of.close();
	}

	void WriteTokens(_STD string& path, _STD vector<Token>& tokens) {
		WriteTokensRaw(path, GetTokenOutBuffer(tokens));
	}

	template<typename T>
	_STD vector<T> MergeVectors(_STD vector<T> first, _STD vector<T> second) {
		_STD vector<T> newVec;
		for (T& t : first)
			newVec.push_back(t);

		for (T& t : second)
			newVec.push_back(t);

		return newVec;
	}

	void CopyString(char** target, const char* original, unsigned int strLen) {
		*target = new char[strLen + 1];
		strcpy(*target, original);
		*(char*)((*target) + strLen) = '\0';
	}

	MRKPatch* ReadPatches(unsigned char* buf, unsigned int* patchLen) {
		struct {
			unsigned int m_Pos;
			unsigned char* m_Buf;

			int readi32() {
				int value = *(int*)(m_Buf + m_Pos);
				m_Pos += sizeof(int);

				return value;
			}

			_STD string readstr() {
				int len = readi32();
				_STD string str;

				for (int i = 0; i < len; i++)
					str += *(char*)(m_Buf + m_Pos + i);

				m_Pos += len;
				return str;
			}
		} stack{
			0,
			buf
		};

		int sanity = stack.readi32();
		MRKLog(concat("sanity=", sanity));

		if (sanity != 0xDEAD999)
			_STD _Xruntime_error("Invalid sanity");

		int patchCount = stack.readi32();
		MRKLog(concat("patch count=", patchCount));
		if (!patchCount)
			return 0;

		*patchLen = patchCount;

		MRKPatch* patches = new MRKPatch[patchCount];
		for (int i = 0; i < patchCount; i++) {
			MRKPatch patch{
				stack.readi32()
			};

			_STD string owner = stack.readstr();
			MRKLog(concat("owner=", owner));
			CopyString(&patch.Owner, owner.c_str(), owner.size());

			int ctxCount = stack.readi32();
			MRKLog(concat("ctx count=", ctxCount));

			for (int j = 0; j < 2; j++) {
				char*** local = (char***)(((uintptr_t)&patch) + sizeof(int) + sizeof(void*) * (j + 1));
				*local = new char* [ctxCount];

				for (int k = 0; k < ctxCount; k++) {
					_STD string localctx = stack.readstr();
					CopyString(&(*local)[k], localctx.c_str(), localctx.size());
				}
			}

			patches[i] = patch;
		}

		return patches;
	}

	extern "C" __declspec(dllexport) void MRKApplyPatches(char* projectPath, unsigned char* buf, unsigned int bufLen) {
		char __path[MAX_PATH];
		DWORD mwf = GetModuleFileNameA(0, __path, MAX_PATH);
		mrks string spath(__path, mwf);

		SYSTEMTIME stime;
		GetSystemTime(&stime);
		spath = concat(spath.substr(0, spath.find_last_of('\\')), "\\", stime.wDay, "_", stime.wMonth,
			" [", stime.wHour, "_", stime.wMinute, "_", stime.wSecond, ".txt");
		MRKSetLogPath(spath);

		MRKLog("MRKApplyPatches start", true, false);

		_STD string __log = "MRKApplyPatches start\n";

		unsigned int len;
		MRKPatch* patches = ReadPatches(buf, &len);

		for (size_t i = 0; i < len; i++) {
			LOGN(concat("Patch ", i));
			MRKPatch* patch = &patches[i];
			LOGN(concat("patch=", patch));

			if (patch->Type == MRK_PATCH_NONE)
				continue;

			_STD vector<_STD string> occs;
			for (_STD filesystem::directory_entry iter : _STD filesystem::recursive_directory_iterator(projectPath)) {
				if (iter.is_directory())
					continue;
				
				_STD string str = iter.path().filename().u8string();
				if (!str.find(patch->Owner)) {
					LOGN(concat("Found file: ", str));

					occs.push_back(iter.path().u8string());
				}
			}

			if (occs.empty()) {
				LOGN(concat("Cannot find file: ", patch->Owner));

				continue;
			}

			if (occs.size() > 1) {
				LOG("Found ");
				LOG(occs.size());
				LOGN(" occs, using 0");

				LOGN(concat("Found ", occs.size(), " occs, using 0"));

				LOGN("DATA: ");
				for (_STD string& occ : occs) {
					LOGN(occ);
				}
			}

			_STD string realOwner = occs[0];

			LOGN(concat("Reading ", realOwner));

			FILE* handle = fopen(realOwner.c_str(), "rb");
			fseek(handle, 0, SEEK_END);
			long len = ftell(handle);
			fseek(handle, 0, SEEK_SET);
			char* contents = (char*)malloc(len + 1);
			fread(contents, len, 1, handle);
			fclose(handle);

			contents[len] = 0;
			_STD string scontent(contents);

			free(contents);

			_STD vector<Token> tokens = Tokens::Collect(scontent, true);
			LOGN(concat("Tokenized, n=", tokens.size()));

			if (patch->Type & MRK_PATCH_METHOD_SIG) {
				//locate sig
				_STD string oldSig = patch->Ctx[MRK_PATCH_CTX_SIG];
				LOGN(concat("Finding sig [", oldSig, ']'));

				_STD vector<Token> tokenizedSig = Tokens::Collect(oldSig, true);

				_STD vector<size_t> sigIndices;
				for (size_t j = 0; j < tokens.size(); j++) {
					Token* token = &tokens[j];
					
					bool dirty = false;
					for (size_t k = 0; k < tokenizedSig.size(); k++) {
						Token* __token = &tokenizedSig[k];
						if (token->Kind != __token->Kind || token->ContextualKind != __token->ContextualKind 
							|| TokenCmpDirect(*token, *__token)) {
							dirty = true;
							break;
						}
					}

					if (!dirty) {
						LOGN(concat("Found sig at ", j));

						sigIndices.push_back(j);
					}
				}

				if (sigIndices.empty()) {
					LOGN(concat("Cannot find sig [", oldSig, ']'));
					continue;
				}

				LOGN(concat("Found ", sigIndices.size(), " sigs, using 0"));
				LOGN("DATA: ");
				for (size_t& idx : sigIndices) {
					LOGN(idx);
				}

				//work on sig
				_STD string newSig = patch->New[MRK_PATCH_CTX_SIG];
				_STD vector<Token> newTokenizedSig = Tokens::Collect(newSig, true);
				ReplaceSig(&tokens, sigIndices[0], newTokenizedSig.size(), newTokenizedSig);

				LOGN("Replaced sigs!");
			}

			if (patch->Type & MRK_PATCH_METHOD_BODY) {
				_STD string oldSig = patch->Ctx[MRK_PATCH_CTX_BODY];
				LOGN(concat("Finding sig [", oldSig, ']'));

				_STD vector<Token> tokenizedSig = Tokens::Collect(oldSig, true);

				_STD vector<size_t> sigIndices;
				for (size_t j = 0; j < tokens.size(); j++) {
					Token* token = &tokens[j];

					bool dirty = false;
					for (size_t k = 0; k < tokenizedSig.size(); k++) {
						Token* __token = &tokenizedSig[k];
						if (token->Kind != __token->Kind || token->ContextualKind != __token->ContextualKind
							|| TokenCmpDirect(*token, *__token)) {
							dirty = true;
							break;
						}
					}

					if (!dirty) {
						LOGN(concat("Found sig at ", j));

						sigIndices.push_back(j);
					}
				}

				if (sigIndices.empty()) {
					LOGN(concat("Cannot find sig [", oldSig, ']'));
					continue;
				}

				LOGN(concat("Found ", sigIndices.size(), " sigs, using 0"));
				LOGN("DATA: ");
				for (size_t& idx : sigIndices) {
					LOGN(idx);
				}

				//m gud
				size_t __off = 0;
				size_t fparam;
				do
					fparam = sigIndices[0] + tokenizedSig.size() + __off++;
				while (tokens[fparam].Value.CharValue != '(');

				struct { int b; int e; int val() { return b - e; } } pEscapeStack, bEscapeStack;

				do {
					Token* curr = &tokens[fparam++];
					if (curr->Value.CharValue == '(')
						pEscapeStack.b++;

					if (curr->Value.CharValue == ')')
						pEscapeStack.e++;
				} while (pEscapeStack.val());

				while (tokens[fparam].Value.CharValue != '{') {
					fparam++;
				}

				size_t __btk = fparam;

				do {
					Token* curr = &tokens[fparam++];
					if (curr->Value.CharValue == '{')
						bEscapeStack.b++;

					if (curr->Value.CharValue == '}')
						bEscapeStack.e++;
				} while (bEscapeStack.val());

				size_t __etk = fparam - 1;

				SubVec<Token> pre(tokens, 0, __btk - 1);
				SubVec<Token> post(tokens, fparam, tokens.size() - fparam);

				_STD string newSig = patch->New[MRK_PATCH_CTX_BODY];
				LOGN(concat("Processing sig:\n", newSig));

				_STD vector<Token> newSigTk = Tokens::Collect(newSig, true);

				tokens = MergeVectors<Token>(MergeVectors<Token>(pre, newSigTk), (_STD vector<Token>)post);
			}

			LOGN("Writing patch");
			WriteTokens(realOwner, tokens);
		}
	}
}