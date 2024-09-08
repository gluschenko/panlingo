#ifndef FAKE_RPOTOBUF_H
#define FAKE_RPOTOBUF_H

#include <cassert>
#include <map>
#include <string>
#include <vector>
#include <memory>

using std::map;
using std::pair;
using std::string;
using std::vector;

class Parameter
{
public:
    Parameter()
    {
        _name = "";
        _value = "";
    }
    ~Parameter() = default;

    std::string name() const
    {
        return _name;
    }

    void set_name(const std::string &name)
    {
        _name = std::string(name);
    }

    std::string value() const
    {
        return _value;
    }

    void set_value(const std::string &value)
    {
        _value = std::string(value);
    }

private:
    std::string _name;
    std::string _value;
};

class Sentence
{
public:
    Sentence()
    {
        _text = "";
    }
    ~Sentence() = default;

    std::string text() const
    {
        return _text;
    }

    void set_text(const std::string &text)
    {
        _text = text;
    }

private:
    std::string _text;
};

class TaskInput
{
public:
    TaskInput()
    {
        _name = "";
        _value = "";
    }
    ~TaskInput() = default;

    std::string name() const
    {
        return _name;
    }

    void set_name(const std::string &name)
    {
        _name = name;
    }

    int file_format_size() const
    {
        return 0;
    }

    std::string file_format(int /*index*/) const
    {
        return "default_file_format";
    }

    void add_file_format(const std::string &format)
    {
        // фиктивная реализация
    }

    int record_format_size() const
    {
        return 0;
    }

    std::string record_format(int /*index*/) const
    {
        return "default_record_format";
    }

    void add_record_format(const std::string &format)
    {
    }

    class Part
    {
    public:
        Part() = default;
        ~Part() = default;

        std::string file_pattern() const
        {
            return "default_file_pattern";
        }
    };

    int part_size() const
    {
        return 1; // вернем один, чтобы использовать часть
    }

    const Part &part(int /*index*/) const
    {
        static Part default_part;
        return default_part;
    }

private:
    std::string _name;
    std::string _value;
};

class TaskSpec
{
public:
    TaskSpec()
    {
        _inputs = std::vector<std::shared_ptr<TaskInput>>();
        _parameters = std::vector<std::shared_ptr<Parameter>>();
    }
    ~TaskSpec() = default;

    class Parameter
    {
    public:
        Parameter()
        {
            _name = "";
            _value = "";
        }
        ~Parameter() = default;

        std::string name() const
        {
            return _name;
        }

        void set_name(const std::string &name)
        {
            _name = std::string(name);
        }
        std::string value() const
        {
            return _value;
        }

        void set_value(const std::string &value)
        {
            _value = std::string(value);
        }

    private:
        std::string _name;
        std::string _value;
    };

    int input_size() const
    {
        return _inputs.size();
    }

    const TaskInput &input(int index) const
    {
        return *_inputs.at(index);
    }

    TaskInput *mutable_input(int index)
    {
        return _inputs.at(index).get();
    }

    TaskInput *add_input()
    {
        _inputs.push_back(std::make_shared<TaskInput>());
        return _inputs.back().get();
    }

    int parameter_size() const
    {
        return _parameters.size();
    }

    const Parameter &parameter(int index) const
    {
        return *_parameters.at(index);
    }

    Parameter *mutable_parameter(int index)
    {
        return _parameters.at(index).get();
    }

    Parameter *add_parameter()
    {
        _parameters.push_back(std::make_shared<Parameter>());
        return _parameters.back().get();
    }

private:
    std::vector<std::shared_ptr<TaskInput>> _inputs;
    std::vector<std::shared_ptr<Parameter>> _parameters;
};

class FeatureFunctionDescriptor
{
public:
    FeatureFunctionDescriptor()
    {
        _name = "";
        _type = "";
        _argument = 0;
        _features = std::vector<std::shared_ptr<FeatureFunctionDescriptor>>();
        _parameters = std::vector<std::shared_ptr<Parameter>>();
    }
    ~FeatureFunctionDescriptor() = default;

    std::string name() const
    {
        return _name;
    }

    void set_name(const std::string &name)
    {
        _name = std::string(name);
    }

    bool has_argument() const
    {
        return _argument != 0;
    }

    int argument() const
    {
        return _argument;
    }

    void set_argument(int argument)
    {
        _argument = argument;
    }

    std::string type() const
    {
        return _type;
    }

    void set_type(const std::string &type)
    {
        _type = std::string(type);
    }

    int feature_size() const
    {
        return _features.size();
    }

    FeatureFunctionDescriptor *mutable_feature(int index)
    {
        return _features.at(index).get();
    }

    int parameter_size() const
    {
        return _parameters.size();
    }

    Parameter parameter(int index) const
    {
        return *_parameters.at(index);
    }

    Parameter *add_parameter()
    {
        _parameters.push_back(std::make_shared<Parameter>());
        return _parameters.back().get();
    }

    FeatureFunctionDescriptor *add_feature()
    {
        _features.push_back(std::make_shared<FeatureFunctionDescriptor>());
        return _features.back().get();
    }

    const FeatureFunctionDescriptor &feature(int index) const
    {
        return *_features.at(index);
    }

private:
    std::string _name;
    std::string _type;
    int _argument;
    std::vector<std::shared_ptr<FeatureFunctionDescriptor>> _features;
    std::vector<std::shared_ptr<Parameter>> _parameters;
};

class FeatureExtractorDescriptor
{
public:
    FeatureExtractorDescriptor()
    {
        _name = "";
        _type = "";
        _argument = 0;
        _features = std::vector<std::shared_ptr<FeatureFunctionDescriptor>>();
        _parameters = std::vector<std::shared_ptr<Parameter>>();
    }
    ~FeatureExtractorDescriptor() = default;

    std::string name() const
    {
        return _name;
    }

    void set_name(const std::string &name)
    {
        _name = std::string(name);
    }

    bool has_argument() const
    {
        return _argument != 0;
    }

    int argument() const
    {
        return _argument;
    }

    void set_argument(int argument)
    {
        _argument = argument;
    }

    std::string type() const
    {
        return _type;
    }

    void set_type(const std::string &type)
    {
        _type = std::string(type);
    }

    int feature_size() const
    {
        return _features.size();
    }

    FeatureFunctionDescriptor *mutable_feature(int index)
    {
        return _features.at(index).get();
    }

    int parameter_size() const
    {
        return _parameters.size();
    }

    const Parameter &parameter(int index) const
    {
        return *_parameters.at(index);
    }

    FeatureFunctionDescriptor *add_feature()
    {
        _features.push_back(std::make_shared<FeatureFunctionDescriptor>());
        return _features.back().get();
    }

    const FeatureFunctionDescriptor &feature(int index) const
    {
        return *_features.at(index);
    }

private:
    std::string _name;
    std::string _type;
    int _argument;
    std::vector<std::shared_ptr<FeatureFunctionDescriptor>> _features;
    std::vector<std::shared_ptr<Parameter>> _parameters;
};

#endif // FAKE_RPOTOBUF_H
